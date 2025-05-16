﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qiniu.Util
{
    /// <summary>
    /// JSON 序列化和反序列化帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 默认的 JSON 序列化选项
        /// </summary>
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        /// <summary>
        /// 将对象序列化为 JSON 字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>JSON 字符串</returns>
        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为指定类型的对象
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <param name="type">目标类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object? Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, DefaultOptions);
        }
    }
}
