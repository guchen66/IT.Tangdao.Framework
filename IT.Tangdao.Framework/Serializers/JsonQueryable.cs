using IT.Tangdao.Framework.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Serializers
{
    /// <summary>
    /// 基于 Newtonsoft.Json 的配置查询封装。
    /// 三种形态：
    ///   JsonQueryable.Current["SqlConnection:ConnectionString"]      // C
    ///   JsonQueryable.GetSection("SqlConnection")["ConnectionString"] // A
    ///   JsonQueryable.GetConfig["SqlConnection"]("ConnectionString")  // B
    /// </summary>
    public class JsonQueryable
    {
        private static JObject _root;

        /// <summary>单例，避免每次读盘。改盘后调用 Reload()</summary>
        private static readonly JsonQueryable _instance = new JsonQueryable();

        private JsonQueryable() { }

        /// <summary>形态 1：根索引器，键支持 "A:B:C" 冒号多级</summary>
        public static JsonQueryable Current => _instance;

        /// <summary>形态 1 的索引器实现</summary>
        public string this[string path]
        {
            get
            {
                var token = Resolve(path);
                return token?.Value<string>();
            }
        }

        /// <summary>形态 2：方法给节，返回带索引器的节对象</summary>
        public static SectionView GetSection(string section)
        {
            var token = Resolve(section);
            return token is null ? null : new SectionView(token);
        }

        /// <summary>形态 3：静态入口给节，再“调用”给键</summary>
        public static SectionAccessor GetConfig { get; } = new SectionAccessor();


        /// <summary>手动重新加载配置文件</summary>
        public static void Reload(string fileName = "AppConfig.json")
        {
            var text = File.Exists(fileName) ? File.ReadAllText(fileName) : "{}";
            _root = JObject.Parse(text);
        }

        /// <summary>内部：按冒号路径逐级定位 JToken</summary>
        internal static JToken Resolve(string path)
        {
            if (_root is null) Reload();
            if (string.IsNullOrEmpty(path)) return _root;

            JToken cur = _root;
            foreach (var part in path.Split(':'))
            {
                if (cur is null) return null;
                cur = cur[part];
            }
            return cur;
        }
    }

    /// <summary>形态 2 的索引器载体：GetSection("节")["键"]</summary>
    public sealed class SectionView
    {
        private readonly JToken _token;
        public SectionView(JToken token) => _token = token;

        /// <summary>给键（单级）。传空串返回该节自身的 Value</summary>
        public string this[string key] =>
            string.IsNullOrEmpty(key) ? _token.Value<string>() : _token[key]?.Value<string>();

        /// <summary>该节自身的值</summary>
        public string Value => _token.Value<string>();
    }

    /// <summary>形态 3 的索引器载体：GetConfig["节"]("键")</summary>
    public sealed class SectionAccessor
    {
        /// <summary>给节（支持冒号多级），返回委托</summary>
        public ConfigGetter this[string section] => key =>
        {
            var token = JsonQueryable.Resolve(section);
            if (token is null) return null;
            return string.IsNullOrEmpty(key)
                ? token.Value<string>()
                : token[key]?.Value<string>();
        };
    }
}
