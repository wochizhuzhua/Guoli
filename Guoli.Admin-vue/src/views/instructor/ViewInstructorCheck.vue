



<template>
  <section>

    <!--工具条-->
    <el-col :span="24" class="toolbar" style="padding-bottom: 0px;">
      <el-form :inline="true" :model="conditions" @submit.native.prevent="load">

        <el-form-item>
          <el-input v-model="conditions.Name.value" placeholder="指导司机"></el-input>
        </el-form-item>
        <el-form-item>
          <el-date-picker v-model="conditions.StartTime.value" type="daterange" placeholder="抽查时间" align="right">
          </el-date-picker>
        </el-form-item>
        <el-form-item>
          <el-select v-model="conditions.CheckType.value" placeholder="抽查类型">
            <el-option v-for="item in CheckTypeSelectData" :key="item.key" :label="item.key" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-input v-model="conditions.Location.value" placeholder="抽查地点"></el-input>
        </el-form-item>
        <el-form-item>
          <el-select v-model="conditions.DepartmentId.value" placeholder="所属部门">
            <el-option v-for="item in DepartmentIdSelectData" :key="item.Id" :label="item.DepartmentName" :value="item.Id"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" v-on:click="load" icon="search">查询</el-button>
        </el-form-item>
      </el-form>
    </el-col>

    <!-- 列表 -->
    <el-table :data="data" highlight-current-row v-loading="isLoading" id="table">

      <el-table-column type="index" width="80" label="序号"></el-table-column>

      <el-table-column prop="Name" label="指导司机"></el-table-column>
      <el-table-column prop="DepartmentName" label="所属单位"></el-table-column>
      <el-table-column prop="Location" label="抽查地点"></el-table-column>
      <el-table-column prop="CheckType" label="抽查类型"></el-table-column>
      <el-table-column prop="ProblemCount" label="发现问题个数" sortable></el-table-column>
      <el-table-column prop="StartTime" label="抽查时间" :formatter="StartTimeFormatter" sortable></el-table-column>
      <el-table-column label="操作" min-width="120">
        <template scope="scope">
          <el-button type="text" @click="showDetail(scope.row)">查看详情</el-button>
        </template>
      </el-table-column>

    </el-table>

    <!--分页工具条-->
    <el-col :span="24" class="toolbar">
      共
      <span>{{ total }}</span> 条， 每页显示
      <el-select v-model="size" size="small" style="width: 70px;" @change="load">
        <el-option v-for="item in sizes" :value="item" :label="item" :key="item"></el-option>
      </el-select>
      条
      <el-pagination layout="prev, pager, next" @current-change="handlePageChange" :page-size="size" :total="total" style="float:right;">
      </el-pagination>
    </el-col>

    <!-- 弹窗 -->
    <el-dialog :title="(`抽查信息单：${timeFormatter(model.StartTime)}`)" :visible="dialogVisible" :before-close="() => dialogVisible = false">
      <el-form label-width="120px">
        <el-form-item label="指导司机：">
          <span>{{ model.Name }}</span>
        </el-form-item>
        <el-form-item label="抽查时间：">
          <span>{{ `${timeFormatter(model.StartTime)} ~ ${timeFormatter(model.EndTime)}` }}</span>
        </el-form-item>
        <el-form-item label="抽查地点：">
          <span>{{ model.Location }}</span>
        </el-form-item>
        <el-form-item label="抽查类型：">
          <span>{{ model.CheckType }}</span>
        </el-form-item>
        <el-form-item label="抽查内容：">
          <span>{{ model.CheckContent }}</span>
        </el-form-item>
        <el-form-item label="发现问题个数：">
          <span>{{ model.ProblemCount }}</span>
        </el-form-item>
        <el-form-item label="发现的问题：">
          <span>{{ model.Problems }}</span>
        </el-form-item>
        <el-form-item label="处理意见：">
          <span>{{ model.Suggests }}</span>
        </el-form-item>
      </el-form>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false" type="primary">关闭</el-button>
      </div>
    </el-dialog>

  </section>
</template>
<script>

import moment from 'moment';

import server from '@/store/server';
import { timepickerOptions } from '@/utils';

export default {
  data() {
    return {
      isLoading: false,
      data: [],

      // 搜索
      apiUrl: '/Instructor/GetListForVue',
      table: 'ViewInstructorCheck',
      order: 'StartTime',
      desc: true,
      conditions: {
        Name: { value: undefined, type: 'like' },
        StartTime: { value: undefined, type: 'between' },
        CheckType: { value: undefined, type: 'equal' },
        Location: { value: undefined, type: 'like' },
        DepartmentId: { value: undefined, type: 'equal' },
      },
      CheckTypeSelectData: [{ key: '测查', value: '测查' }, { key: '坐岗', value: '坐岗' }, { key: '其他', value: '其他' }],
      DepartmentIdSelectData: [],

      // 分页
      total: 0,
      sizes: [10, 20, 50, 100],
      size: 10,
      page: 1,

      // 弹窗
      dialogVisible: false,
      model: {}
    };
  },

  methods: {
    load: function() {
      let o = {
        page: this.page,
        size: this.size,
        order: this.order,
        desc: this.desc,
        table: this.table,
        conditions: this.conditions
      };
      server.post(this.apiUrl, { json: JSON.stringify(o) }).then(res => {
        let { data } = res;
        if (data) {
          let { total, list } = data;
          this.total = total;
          this.data = list;
        }
      });
    },

    StartTimeFormatter: function(row) { return moment(row.StartTime).format('YYYY-MM-DD'); },

    handlePageChange: function(page) {
      this.page = page;
      this.load();
    },

    timeFormatter(time) {
      let d = moment(time);
      if (d.year() < 2000) {
        return '';
      }

      return d.format('YYYY-MM-DD HH:mm:ss');
    },

    showDetail(model) {
      this.model = model;
      this.dialogVisible = true;
    }
  },

  mounted() {
    server.post('/Common/GetDeparts', {}, this).then(res => {
      let { code, data } = res; this.DepartmentIdSelectData = data;
    });

    this.load();
  }
};
</script>
<style scoped lang="scss">

</style>

