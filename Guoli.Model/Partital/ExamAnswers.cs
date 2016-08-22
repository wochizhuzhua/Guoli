﻿/**  版本信息模板在安装目录下，可自行修改。
* ExamAnswers.cs
*
* 功 能： N/A
* 类 名： ExamAnswers
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2016/7/14 10:42:20   N/A    初版
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
	/// 试题答案表
	/// </summary>
	[Serializable]
	public partial class ExamAnswers
	{
		public ExamAnswers()
		{}
		#region Model
		private int _id;
		private int _questionid;
		private string _answer;
		private bool _isright;
		/// <summary>
		/// 主键
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 试题Id，关联试题表主键
		/// </summary>
		public int QuestionId
		{
			set{ _questionid=value;}
			get{return _questionid;}
		}
		/// <summary>
		/// 答案内容
		/// </summary>
		public string Answer
		{
			set{ _answer=value;}
			get{return _answer;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsRight
		{
			set{ _isright=value;}
			get{return _isright;}
		}
		#endregion Model

	}
}

