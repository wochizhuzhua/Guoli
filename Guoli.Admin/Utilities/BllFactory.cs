﻿using Guoli.Utilities.Helpers;

namespace Guoli.Admin.Utilities
{
    ///// <summary>
    ///// 提供根据条件创建bll层实体对象的方法
    ///// </summary>
    ///// <author>FrancisTan</author>
    ///// <since>2016-07-30</since>
    //public static class BllFactory
    //{
    //    /// <summary>
    //    /// 根据表名创建对应的bll层实体对象
    //    /// </summary>
    //    /// <param name="tableName">目标表名称</param>
    //    /// <returns>继承自BaseBll类型的子类对象</returns>
    //    public static object GetBllInstance(string tableName)
    //    {
    //        var assemblyName = "Guoli.Bll";
    //        var fullName = $"{assemblyName}.{tableName}Bll";

    //        return ReflectorHelper.GetInstance(assemblyName, fullName);
    //    }
    //}
}