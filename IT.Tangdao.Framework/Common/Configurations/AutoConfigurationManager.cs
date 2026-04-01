using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Configurations
{
    public class AutoConfigurationManager
    {
        private static readonly Lazy<AutoConfigurationManager> _instance = new Lazy<AutoConfigurationManager>(() => new AutoConfigurationManager());

        public static AutoConfigurationManager Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        private readonly object _lock = new object();

        private AutoConfigurationManager()
        { }

        //public T Load<T>(string filePath) where T : class, new()
        //{
        //    var key = $"{typeof(T).FullName}|{filePath}";
        //    if (_cache.TryGetValue(key, out var cached))
        //        return (T)cached;

        //    lock (_lock)
        //    {
        //        if (_cache.TryGetValue(key, out var cached2))
        //            return (T)cached2;

        //        var config = LoadFromFile<T>(filePath);
        //        _cache[key] = config;
        //        return config;
        //    }
        //}

        //public void Reload<T>(string filePath) where T : class, new()
        //{
        //    var key = $"{typeof(T).FullName}|{filePath}";
        //    lock (_lock)
        //    {
        //        _cache.TryRemove(key, out _);
        //    }
        //}

        //private T LoadFromFile<T>(string filePath) where T : class, new()
        //{
        //    // 根据文件扩展名选择反序列化方式
        //    var ext = Path.GetExtension(filePath).ToLower();
        //    var content = File.ReadAllText(filePath);

        //    return ext switch
        //    {
        //        ".json" => JsonSerializer.Deserialize<T>(content),
        //        ".xml" => XmlSerializerHelper.Deserialize<T>(content),
        //        ".ini" => IniParserHelper.Parse<T>(content),
        //        _ => throw new NotSupportedException($"不支持的文件格式: {ext}")
        //    };
        //}
    }
}