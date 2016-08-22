﻿/**  版本信息模板在安装目录下，可自行修改。
* ExamFiles.cs
*
* 功 能： N/A
* 类 名： ExamFiles
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/8/12 18:04:27   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace Guoli.Model
{
	/// <summary>
	/// 试题文件表
	/// </summary>
	[Serializable]
	public partial class ExamFiles
	{
		public ExamFiles()
		{}
		#region Model
		private int _id;
		private int _examtypeid;
		private string _filename;
		private string _filedesc="";
		private string _filepath;
		private DateTime _addtime= DateTime.Now;
		private bool _isdelete= false;
		/// <summary>
		/// 主键
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 试题分类Id，关联表ExamType的主键
		/// </summary>
		public int ExamTypeId
		{
			set{ _examtypeid=value;}
			get{return _examtypeid;}
		}
		/// <summary>
		/// 试题文件名称
		/// </summary>
		public string FileName
		{
			set{ _filename=value;}
			get{return _filename;}
		}
		/// <summary>
		/// 试题文件描述
		/// </summary>
		public string FileDesc
		{
			set{ _filedesc=value;}
			get{return _filedesc;}
		}
		/// <summary>
		/// 试题文件地址
		/// </summary>
		public string FilePath
		{
			set{ _filepath=value;}
			get{return _filepath;}
		}
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime AddTime
		{
			set{ _addtime=value;}
			get{return _addtime;}
		}
		/// <summary>
		/// 删除标识
		/// </summary>
		public bool IsDelete
		{
			set{ _isdelete=value;}
			get{return _isdelete;}
		}
		#endregion Model

	}
}

