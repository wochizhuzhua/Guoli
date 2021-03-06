﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Guoli.DbProvider
{
    /// <summary>
    /// 数据库参数帮助类，提供根据不同的数据库类型创建不同的DbParameter对象的方法
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2016-06-30</since>
    public static class DbParameterHelper
    {
        /// <summary>
        /// 根据指定的数据库类型以及初始值创建数据库参数对象
        /// </summary>
        /// <param name="dbType">目标数据库类型</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns>IDataParameter的派生类的一个实例</returns>
        public static IDataParameter CreateParameter(DatabaseType dbType, string paramName, string paramValue)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlParameter(paramName, paramValue);
                case DatabaseType.Oracle:
                    return new OracleParameter(paramName, paramValue);
                case DatabaseType.MySql:
                    throw new ArgumentOutOfRangeException("dbType", dbType, "Mysql is not supported.");
                default:
                    throw new ArgumentOutOfRangeException("dbType", dbType, "Unsupported database type.");
            }
        }
    }
}
