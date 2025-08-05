using IT.Tangdao.Framework.DaoAdmin;
using IT.Tangdao.Framework.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Providers
{
    public class DaoJsonConfigProvider
    {
        private string _jsonFileName;

        // 公共属性，允许外部设置
        public string JsonFileName
        {
            get => _jsonFileName ?? "appsettings.json"; // 默认值逻辑
            set => _jsonFileName = value;
        }

        public DaoJsonConfigProvider()
        {
            JsonFileName = "appsettings.json";
        }

        public IReadResult GetSection(string key)
        {
            var path = DirectoryHelper.SelectDirectoryByName(JsonFileName);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "{}"); // 写入空JSON对象
            }

            // 处理key中的冒号分隔
            string tokenPath = key.Contains(":")
                ? key.Replace(":", ".")
                : key;
            string jsonContent = File.ReadAllText(path);
            JObject jsonObject = JObject.Parse(jsonContent);
            if (jsonObject == null)
            {
                return new IReadResult("转换失败，json数据为空", false);
            }
            JToken valueToken = jsonObject.SelectToken(tokenPath);

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                // 键不存在或值为 null
                return new IReadResult("转换失败，JToken为null", false);
            }
            return new IReadResult(valueToken.ToString(), true);
        }
    }
}