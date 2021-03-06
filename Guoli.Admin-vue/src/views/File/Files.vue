<template>
  <section>

    <!-- 工具条 -->
    <el-row class="toolbar">
      <el-col :span="24">
        <el-button size="small" icon="arrow-left" @click.native="folderBack" v-if="pathStack.length > 0">返回上级目录</el-button>

        <div class="path-container">
          <el-button type="text" class="folder-path" v-for="folder in pathStack" :key="folder.Id" @click.native="pathClick(folder)">{{ folder.TypeName }}</el-button>
        </div>

        <el-button class="btn-right" size="small" type="primary" icon="plus" @click.native="newFolder">新建目录</el-button>
        <el-button class="btn-right" size="small" type="primary" icon="plus" v-if="curParentId > 0" @click.native="addFiles">添加文件</el-button>
        <el-button class="btn-right info" style="margin-right: 16px;" type="text" v-if="curParentId > 0">
          当前目录{{ curDirectory.IsPublic ? '是公共目录' : `由${curDirectory.DepartmentName}负责` }}
        </el-button>
      </el-col>
    </el-row>

    <!-- 文件夹列表 -->
    <el-row :gutter="8" class="content center" :loading="isFileLoading">

      <!-- 没有任何文件或文件夹时 -->
      <el-col :span="24" class="file-empty" v-if="curDirs.length === 0 && curFiles.length === 0">
        <img src="../../assets/empty.png" />
        <p>空空如也(:=</p>
      </el-col>

      <!-- 文件夹 -->
      <el-col :span="4" v-for="folder in curDirs" :key="folder.Id" @click.native="openFolder(folder)">
        <div class="folder">
          <i class="folder-img fa fa-folder-o" aria-hidden="true"></i>
          <div class="file-info">
            <span class="file-name">{{ folder.TypeName }}</span>
            <span class="file-time">{{ `${folder.DepartmentName || ''}-${folder.CreatorName || ''}` }}</span>
            <span class="file-time">{{ folder.CreateTime | moment('YYYY-MM-DD HH:mm:ss') }}</span>
          </div>
          <div class="file-shadow">
            <p>
              <el-button class="file-btn" size="small" icon="edit" @click.native.stop="renameFolder(folder)">修改</el-button>
              <el-button class="file-btn" size="small" icon="delete" @click.native.stop="remove(folder.Id, 1, folder)">删除</el-button>
            </p>
            <span class="file-tip">点击打开</span>
          </div>
        </div>
      </el-col>

      <!-- 文件 -->
      <transition name="fade" v-for="file in curFiles" :key="file.Id">

          <el-col :span="4">
            <div class="folder">
              <a title="点击下载文件" :href="`${base}${file.OriginFilePath || file.FilePath}`" :download="getDownloadFileName(file)">
                <i class="folder-img" :class="getFileIconClass(getFileExtension(file.OriginFilePath || file.FilePath))" aria-hidden="true"></i>
              </a>
              <!-- <div class="file-info">
                  <span class="file-name">{{ file.FileName }}</span>
                  <span class="file-time">{{ `${file.DepartmentName}-${file.CreatorName}` }}</span>
                  <span class="file-time">{{ file.AddTime | moment('YYYY-MM-DD HH:mm:ss') }}</span>
                </div> -->
              <a title="点击下载文件" class="file-info" :href="`${base}${file.OriginFilePath || file.FilePath}`" :download="file.FileName">
                <span class="file-name">{{ getDownloadFileName(file) }}</span>
                <span class="file-time">{{ `${file.DepartmentName}-${file.CreatorName}` }}</span>
                <span class="file-time">{{ file.AddTime | moment('YYYY-MM-DD HH:mm:ss') }}</span>
              </a>
              <div class="file-shadow">
                <p>
                  <el-button class="file-btn" size="small" icon="edit" @click.native.stop="renameFile(file)">重命名</el-button>
                  <el-button class="file-btn" size="small" icon="delete" @click.native.stop="remove(file.Id, 2, file)">删除</el-button>
                </p>
              </div>
            </div>
          </el-col>

        <a :href="downloadUrl" style="display: none;" :download="downloadName"></a>

      </transition>

    </el-row>

    <!-- 添加文件 -->
    <el-dialog title="添加文件" v-model="isUploadVisible">
      <el-upload class="file-upload" ref="upload" :action="uploadPath()" :on-success="uploadSuccess" :before-upload="beforeUpload" drag multiple>
        <i class="el-icon-upload"></i>
        <div class="el-upload__text">将文件拖到此处，或
          <em>点击上传</em>
        </div>
      </el-upload>
    </el-dialog>

    <!-- 重命名 -->
    <el-dialog title="重命名" v-model="renameFormVisible" :close-on-click-modal="false">
      <el-form :model="renameFormModel" label-width="100px" :rules="renameFormRules" @submit.native.prevent="renameFormSubmit" ref="renameForm">
        <el-form-item :label="renameFormModel.type === 1 ? '目录名称:' : '文件名称:'" prop="name">
          <el-input v-model="renameFormModel.name" auto-complete="off"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native="renameFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="renameFormSubmit" :loading="renameFormLoading">提交</el-button>
      </div>
    </el-dialog>

    <!-- 新建目录 -->
    <el-dialog title="新建目录" v-model="newFormVisible" :close-on-click-modal="false">
      <el-alert title="关于目录公开性" type="info" description="若创建目录并将其指定为私有文件夹，则在此文件夹下，只有责任部门的管理员账户才有权限对文件进行管理（添加、修改或删除），反之，则所有部门均拥有权限在此目录下管理文件。" show-icon style="margin-bottom: 30px;" v-if="user.IsSuper && curParentId === 0"></el-alert>

      <el-form :model="newFormModel" label-width="100px" :rules="newFormRules" @submit.native.prevent="newFormSubmit" ref="newForm">
        <el-form-item label="目录名称:" prop="name">
          <el-input v-model="newFormModel.name" auto-complete="off"></el-input>
        </el-form-item>

        <el-form-item prop="isPublic" label="公开性：" v-if="user.IsSuper && curParentId === 0">
          <el-radio-group v-model.number="newFormModel.isPublic" @change="onPublicChange">
            <el-radio-button :label="false">私有文件夹</el-radio-button>
            <el-radio-button :label="true">公共文件夹</el-radio-button>
          </el-radio-group>
        </el-form-item>

        <el-form-item prop="hidden" label="是否仅本部门可见：" v-if="user.IsSuper && curParentId === 0 && !newFormModel.isPublic">
          <el-radio-group v-model.number="newFormModel.hidden">
            <el-radio-button :label="true">是</el-radio-button>
            <el-radio-button :label="false">否</el-radio-button>
          </el-radio-group>
        </el-form-item>

        <!-- 超级管理员添加目录需指定责任部门 -->
        <el-form-item prop="depart" label="责任部门：" v-if="user.IsSuper && curParentId === 0">
          <el-cascader :options="cascaderOptions" v-model="newFormModel.depart" clearable expand-trigger="hover" placeholder="请选择部门" :show-all-levels="true"></el-cascader>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native="newFormVisible = false">取消</el-button>
        <el-button type="primary" @click.native="newFormSubmit" :loading="newFormLoading">提交</el-button>
      </div>
    </el-dialog>

  </section>
</template>

<script>
import NProgress from "nprogress";
import _ from "underscore";
import server from "@/store/server";
import local from "@/store/local";
import { fileIcons, rules } from "@/utils";

export default {
  data() {
    return {
      base: server.base,
      user: {},

      isFileLoading: false,
      data: [],
      curParentId: 0,
      curDirectory: {},
      curDirs: [],
      curFiles: [],
      pathStack: [],
      fileExtensions: [],

      // 文件/目录重合表单数据
      renameFormVisible: false,
      renameFormLoading: false,
      renameFormModel: { id: 0, name: "", origin: "", type: 1 }, // type为1表示对目录重命名，2表示对文件重命名
      renameFormRules: { name: [rules.required, rules.getMaxRule(50)] },
      renameOriginModel: {},

      // 新建文件夹表单数据
      newFormVisible: false,
      newFormLoading: false,
      newFormModel: { name: "", depart: [], isPublic: false, hidden: false },
      newFormRules: {
        name: [rules.required, rules.getMaxRule(50)],
        depart: [
          {
            required: true,
            type: "array",
            message: "请选择责任部门",
            trigger: "change"
          }
        ]
      },
      newFormOrigin: {},

      // 上传文件
      isUploadVisible: false,

      // 部门级联选择数据
      cascaderOptions: [],
      departs: []
    };
  }, // end data()

  methods: {
    loadFiles: function() {
      this.isFileLoading = true;
      NProgress.start();

      return server.post("/Files/GetDirecAndFiles", {}, this).then(res => {
        this.isFileLoading = false;
        NProgress.done();
        this.data = res.data;
      });
    },

    getCurrentFiles: function() {
      let _this = this;
      _this.curDirs = _this.data.directories.filter(
        (item, index) => item.ParentId === _this.curParentId
      );
      _this.curFiles = _this.data.files.filter(
        (item, index) => item.TypeId === _this.curParentId
      );
    },

    openFolder: function(folder) {
      this.curParentId = folder.Id;
      this.curDirectory = folder;

      this.getCurrentFiles();
      this.pathStack.push(folder);
    },

    // 文件路径点击事件
    pathClick: function(folder) {
      let p = this.pathStack;
      for (let i = p.length - 1; i >= 0; i--) {
        if (p[i].Id !== folder.Id) {
          p.splice(i, 1);
        } else {
          this.curParentId = folder.Id;
          this.curDirectory = folder;
          this.getCurrentFiles();
          break;
        }
      }
    },

    folderBack: function() {
      let p = this.pathStack;
      p.pop();
      this.curParentId = p.length === 0 ? 0 : p[p.length - 1].Id;
      this.curDirectory = _.last(p) || {};
      this.getCurrentFiles();
    },

    getFileExtenExtension: function(filename) {
      return filename.substr(filename.lastIndexOf("."));
    },

    getFileIconClass: extension => {
      return fileIcons[extension] || "fa fa-file-o";
    },

    // 点击添加目录触发的事件
    newFolder: function() {
      // 顶级目录仅超级管理员可添加目录
      let isSuper = this.user.IsSuper;
      if (this.curParentId === 0) {
        if (!isSuper) {
          this.$message({
            type: "error",
            message: "抱歉，只有超级管理员才能在顶级目录中添加目录(:="
          });
          return;
        }
      } else {
        if (!isSuper) {
          let c = this.curDirectory;
          console.info(c.DepartmentName);
          // 在非公共目录中，只有超级管理员
          // 以及责任部门的管理员可以创建目录
          if (!c.IsPublic && this.user.DepartmentId !== c.DepartmentId) {
            this.$message({
              type: "error",
              message: `抱歉，当前目录的责任部门是${c.DepartmentName}，您没有操作权限(:=`
            });
            return;
          }
        }
      }

      this.newFormVisible = true;
      this.newFormModel.name = "";
      this.newFormModel.depart = [];
      this.newFormModel.isPublic = false;
      this.newFormModel.hidden = false;
      this.newFormOrigin = {};
    },

    // 添加目录表单中的公开性选项值发生变化时
    // 若选择了公共文件夹，则需将model中的
    // hidden值设置为false
    onPublicChange: function(isPublic) {
      if (isPublic) {
        this.newFormModel.hidden = false;
      }
    },

    // 添加目录表单提交
    newFormSubmit: function() {
      this.$refs.newForm.validate(valid => {
        if (valid) {
          let m = this.newFormModel;
          // 目录的责任部门要么由超级管理员指定
          // 要么默认为当前登录用户所在部门
          let departmentId =
            _.last(m.depart) ||
            this.curDirectory.DepartmentId ||
            this.user.DepartmentId;
          let origin = this.newFormOrigin;
          let model = {
            Id: origin.Id || 0,
            TypeName: m.name,
            ParentId: this.curParentId,
            DepartmentId: departmentId,
            IsPublic: m.isPublic,
            Hidden: m.hidden,
            CreatorId: this.user.Id
          };

          // 若满足下面的条件
          // 则说明文件信息没有被改变
          // 不需要提交服务器
          if (
            model.TypeName === origin.TypeName &&
            model.DepartmentId === origin.DepartmentId &&
            model.IsPublic === origin.IsPublic
          ) {
            return;
          }

          this.newFormLoading = true;
          NProgress.start();
          server
            .post(
              "/Files/AddDirectory",
              { directory: JSON.stringify(model) },
              this
            )
            .then(res => {
              this.newFormLoading = false;
              NProgress.done();

              let { code, id } = res;
              let successMsg = model.Id > 0 ? `目录修改成功(:=` : "目录添加成功(:=";
              let failureMsg =
                model.Id > 0 ? "目录修改失败，请稍后重试(:=" : "目录添加失败，请稍后重试(:=";
              if (code === 119) {
                this.$message({ type: "error", message: "已存在相同名称的目录(:=" });
              } else if (code === 116) {
                // 目录添加成功
                this.$message({ type: "success", message: successMsg });
                this.newFormVisible = false;

                let departName = _.find(
                  this.departs,
                  item => item.Id === model.DepartmentId
                ).DepartmentName;
                if (model.Id > 0) {
                  // 将更新同步到本地
                  origin.TypeName = model.TypeName;
                  origin.IsPublic = model.IsPublic;
                  origin.Hidden = model.Hidden;
                  origin.DepartmentId = model.DepartmentId;
                  origin.DepartmentName = departName;
                } else {
                  // 将添加的目录同步到本地
                  // 避免重新从服务器加载
                  model.Id = id;
                  model.DepartmentName = departName;
                  this.addLocal(model, 1);
                }
              } else {
                this.$message({ type: "error", message: failureMsg });
              }
            });
        }
      });
    },

    // 点击添加文件按钮触发
    addFiles: function() {
      // 在私有文件目录中
      // 若当前登录用户不是超级管理员
      // 并且非当前目录责任部门的管理员
      // 则无权限添加文件
      if (
        !this.user.IsSuper &&
        !this.curDirectory.IsPublic &&
        this.user.DepartmentId !== this.curDirectory.DepartmentId
      ) {
        this.$message({
          type: "error",
          message: `抱歉，当前目录是由${this.curDirectory.DepartmentName}负责的，您无权限添加文件(:=`
        });
        return;
      }

      this.isUploadVisible = true;
    },

    // 动态获取文件上传地址，因为需要通过url传递动态参数
    uploadPath: function() {
      return `${server.base}/Files/AddFiles?token=${local.getItem(
        "token"
      )}&fileType=3&typeId=${this.curParentId}&depart=${this.curDirectory
        .DepartmentId}`;
    },

    beforeUpload: function(file) {
      let ext = this.getFileExtension(file.name);
      if (!_.contains(this.fileExtensions, ext)) {
        this.$error(`不允许上传像《${file.name}》这样格式的文件(:=`);
        return false;
      }
    },

    getFileExtension: function(fileName) {
      return fileName.substr(fileName.lastIndexOf("."));
    },

    // 文件上传成功
    uploadSuccess: function(response, file, fileList) {
      let { msg, fileModel } = response;
      if (msg.code === 100) {
        this.data.files.push(fileModel);
        this.curFiles.push(fileModel);
        this.$notify({
          type: "success",
          title: "提示",
          message: `《${file.name}》上传成功！`
        });
      } else if (msg.code === 120) {
        this.$notify({ type: "error", message: `《${file.name}》已存在(:=` });
      } else {
        this.$notify({ type: "error", message: `《${file.name}》上传失败(:=` });
      }
    },

    getDownloadFileName: function(file) {
      if (file.OriginFilePath) {
        var originExt = this.getFileExtension(file.OriginFilePath);
        var ext = this.getFileExtension(file.FilePath);
        return file.FileName.replace(ext, originExt);
      } else {
        return file.FileName;
      }
    },

    addLocal: function(model, type) {
      if (type === 1) {
        this.data.directories.push(model);
        this.curDirs.push(model);
      } else if (type === 2) {
        this.data.file.push(model);
        this.curFiles.push(model);
      }
    },

    // 点击目录中的修改按钮触发
    renameFolder: function(folder) {
      // 权限验证
      if (folder.IsPublic) {
        if (!this.user.IsSuper) {
          // 公共目录仅超级管理员有修改权限
          this.$message({
            type: "error",
            message: "抱歉，只有超级管理员才有对公共目录进行修改的权限(:="
          });
          return;
        }
      } else {
        // 私有目录仅超级管理员和责任部门管理员有修改权限
        if (
          !this.user.IsSuper &&
          this.user.DepartmentId !== folder.DepartmentId
        ) {
          this.$message({
            type: "error",
            message: `抱歉，此目录是由${folder.DepartmentName}负责的，您没有修改权限(:=`
          });
          return;
        }
      }

      let m = this.newFormModel;
      m.name = folder.TypeName;
      m.isPublic = folder.IsPublic;
      m.hidden = folder.Hidden;

      let selected = [];
      this.findParents(this.departs, folder.DepartmentId, selected);
      m.depart = selected;

      this.newFormOrigin = folder;
      this.newFormVisible = true;
    },

    // 点击文件中的重命名按钮触发
    renameFile: function(file) {
      if (!this.user.IsSuper && !this.user.DepartmentId !== file.DepartmentId) {
        this.$message({
          type: "error",
          message: `抱歉，此文件是由${file.DepartmentName}上传的，您无权限修改(:=`
        });
        return;
      }

      this.renameFormVisible = true;
      this.renameFormModel.id = file.Id;
      this.renameFormModel.type = 2;
      this.renameOriginModel = file;

      let name = file.FileName.replace(file.FileExtension, "");
      this.renameFormModel.name = name;
      this.renameFormModel.origin = name;
    },

    renameFormSubmit: function() {
      this.$refs.renameForm.validate(valid => {
        if (valid) {
          let m = this.renameFormModel;
          let changed = m.name !== m.origin;
          if (changed) {
            this.renameFormLoading = true;
            NProgress.start();
            let { id, type } = this.renameFormModel;
            let name =
              this.renameFormModel.name + this.renameOriginModel.FileExtension;
            // 文件重命名
            server.post("/Files/Rename", { id, name, type }, this).then(res => {
              let { code } = res;
              if (code === 100) {
                this.$message({
                  type: "success",
                  title: "提示",
                  message: `已成功将《${m.origin}》重命名为《${m.name}》(:=`,
                  duration: 3000
                });
                this.renameOriginModel.FileName = name;
                this.renameFormLoading = false;
                this.renameFormVisible = false;
                NProgress.done();
              }
            });
          } else {
            this.renameFormVisible = false;
          }
        }
      });
    },

    remove: function(id, type, model) {
      // 对目录或文件的删除权限
      // 仅超级管理员和其责任部门
      // 的管理员账户拥有
      if (!this.user.IsSuper && this.user.DepartmentId !== model.DepartmentId) {
        let msg =
          type === 1
            ? `抱歉，此目录是由${model.DepartmentName}负责的，您无权限删除(:=`
            : `抱歉，此文件是由${model.DepartmentName}上传的，您无权限删除(:=`;
        this.$message({ type: "error", message: msg });
        return;
      }

      let text = type === 1 ? `将删除此目录及其所有的子目录和子文件，是否确定删除？` : `文件删除后将无法恢复，是否删除？`;
      this.$confirm(text)
        .then(() => {
          server.post("/Files/Delete", { id, type }, this).then(res => {
            let { code } = res;
            if (code === 100) {
              this.$message({
                type: "success",
                message: "删除成功(:=",
                duration: 3000
              });
              this.removeFromLocalData(id, type);
            }
          });
        })
        .catch(() => {
          /* 取消 */
        });
    },

    findIndex: (arr, id) => {
      for (let i = 0; i < arr.length; i++) {
        if (arr[i].Id === id) {
          return i;
        }
      }

      return -1;
    },

    removeFromLocalData: function(id, type) {
      let index = -1;

      if (type === 1) {
        index = this.findIndex(this.data.directories, id);
        this.data.directories.splice(index, 1);

        index = this.findIndex(this.curDirs, id);
        if (index >= 0) {
          this.curDirs.splice(index, 1);
        }
      } else {
        index = this.findIndex(this.data.files, id);
        this.data.files.splice(index, 1);

        index = this.findIndex(this.curFiles, id);
        if (index >= 0) {
          this.curFiles.splice(index, 1);
        }
      }
    },

    //************部门选择器数据处理函数*************
    //
    loadDeparts: function() {
      server.post("/Common/GetDeparts", {}, this).then(res => {
        let { code, data } = res;
        this.departs = data;
        this.cascaderOptions = this.findChildren(data, 0);
      });
    },

    findChildren: function(array, parentId) {
      let sub = _.filter(array, item => item.ParentId === parentId);
      if (sub.length === 0) {
        return [];
      }

      let result = [];
      sub.forEach(item => {
        // 递归遍历具有层级关系的部门列表
        // 并将其包装为符合el-cascader组件所需要的数据格式后返回
        let children = this.findChildren(array, item.Id);

        let obj = { label: item.DepartmentName, value: item.Id };
        if (children.length > 0) {
          obj.children = children;
        }
        result.push(obj);
      });

      return result;
    },

    findParents: function(array, id, result) {
      let obj = _.find(array, elem => elem.Id === id);
      result.splice(0, 0, id);

      if (obj.ParentId === 0) {
        result.splice(0, 0, obj.ParentId);
        return;
      }

      this.findParents(array, obj.ParentId, result);
    }
  }, // end methods

  mounted() {
    this.loadFiles().then(res => this.getCurrentFiles());
    this.user = local.getItem("user");
    this.loadDeparts();

    server.post("/Files/GetFileExtensions", {}, this).then(res => {
      this.fileExtensions = res.data;
    });
  }
};
</script>

<style scoped lang="scss">
@import "../sass/mixins.scss";

$folderColor: #a3ddff;
$folderContrastColor: #ed5e07;
$pdfColor: #f14b37;
$blue: #20a0ff;

.toolbar {
  border-bottom: 1px solid $borderColor;

  .btn-right {
    float: right;
    margin-left: 8px;

    &.info {
      margin-right: 16px;
      cursor: default;
      color: #666;
    }
  }
}

.path-container {
  display: inline-block;
  margin-left: 8px;

  .folder-path {
    &::after {
      content: ">";
      color: #bfcbd9;
      margin-left: 8px;
    }

    &:last-child {
      cursor: pointer;
      color: #bfcbd9;

      &::after {
        content: "";
      }
    }
  }
}

.file-empty {
  text-align: center;

  p {
    color: $lightSilver;
    font-size: 1.5rem;
    margin: 0;
  }
}

.folder {
  text-align: center;
  padding: 10px;
  cursor: pointer;
  margin-top: 10px;
  position: relative;
  border: 1px solid $borderColor;
  overflow: hidden;
  height: 217px;

  &.active {
    background: $folderColor;

    .folder-img {
      color: #fff;
    }
    .file-info .file-name {
      color: #fff;
    }
    .file-info .file-time {
      color: #fff;
    }
  }

  .file-info {
    display: block;
    text-decoration: none; // padding: 10px 14px 0 14px;
    span {
      display: inline-block;
      margin-top: 10px;

      &:last-child {
        margin-bottom: 0;
      }
    }

    .file-name {
      font-size: 14px;
      color: #324057;
      width: 100%;
      height: 40px;
      line-height: 20px;
      overflow: hidden;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
    }

    .file-time {
      font-size: 14px;
      color: $lightSilver;
    }
  }

  .folder-img {
    width: 100%;
    color: $folderColor;
    font-size: 5rem;
  }

  .file-shadow {
    position: absolute;
    bottom: -96px;
    left: 0;
    padding: 10px 0;
    text-align: center;
    background-color: #000;
    width: 100%;
    opacity: 0.8;
    color: #fff;
    @include prefix(transition, bottom 0.3s);

    .file-btn {
      color: #fff;
      background: #000;
      border: 1px solid #fff;
    }
  }

  &:hover .file-shadow {
    bottom: 0;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: all 0.5s ease-out;
}

.fade-enter,
.fade-leave-to {
  opacity: 0;
}
</style>
