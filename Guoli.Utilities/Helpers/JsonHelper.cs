﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Guoli.Utilities.Helpers
{
    /// <summary>
    /// Json工具类，提供json序列化及反序列化等方法
    /// </summary>
    /// <author>FrancisTan</author>
    /// <since>2016-07-15</since>
    public static class JsonHelper
    {
        private static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Error = (obj, args) => args.ErrorContext.Handled = true
            //Formatting = Formatting.Indented,
            //Converters = new List<JsonConverter> { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } }
        };

        /// <summary>
        /// 将指定对象序列化，并返回指定对象的json字符串表示
        /// </summary>
        /// <param name="value">待序列化的对象</param>
        /// <returns>返回指定对象的json字符串表示</returns>
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, DefaultSettings);
        }

        /// <summary>
        /// 将给定字符串反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">需要反序列化的目标对象的类型</typeparam>
        /// <param name="value">给定的json字符串</param>
        /// <returns>指定类型的实体或者null</returns>
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, DefaultSettings);
        }
    }
}
