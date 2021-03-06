﻿/**  版本信息模板在安装目录下，可自行修改。
* InstructorRouterPosition.cs
*
* 功 能： N/A
* 类 名： InstructorRouterPosition
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/9/28 11:28:58   N/A    初版
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
	/// 路由器地点对照表
	/// </summary>
	[Serializable]
	public partial class InstructorRouterPosition
	{
		public InstructorRouterPosition()
		{}
		#region Model
		private int _id;
		private string _bssid;
        private string _ip;
		private string _location;
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
		/// Mac地址，多个以逗号隔开
		/// </summary>
		public string BssId
		{
			set{ _bssid=value;}
			get{return _bssid;}
		}
		/// <summary>
		/// 实际地址
		/// </summary>
		public string Location
		{
			set{ _location=value;}
			get{return _location;}
		}
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip
        {
            set { _ip = value; }
            get { return _ip; }
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

