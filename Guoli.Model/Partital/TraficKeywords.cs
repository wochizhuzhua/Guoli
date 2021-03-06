﻿/**  版本信息模板在安装目录下，可自行修改。
* TraficKeywords.cs
*
* 功 能： N/A
* 类 名： TraficKeywords
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/7/14 10:42:29   N/A    初版
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
	/// 搜索关键字表
	/// </summary>
	[Serializable]
	public partial class TraficKeywords
	{
		public TraficKeywords()
		{}
		#region Model
		private int _id;
		private string _keywords;
		private int _searchcount=0;
		private DateTime _addtime= DateTime.Now;
		/// <summary>
		/// 主键
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 关键字
		/// </summary>
		public string Keywords
		{
			set{ _keywords=value;}
			get{return _keywords;}
		}
		/// <summary>
		/// 搜索次数
		/// </summary>
		public int SearchCount
		{
			set{ _searchcount=value;}
			get{return _searchcount;}
		}
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime AddTime
		{
			set{ _addtime=value;}
			get{return _addtime;}
		}
		#endregion Model

	}
}

