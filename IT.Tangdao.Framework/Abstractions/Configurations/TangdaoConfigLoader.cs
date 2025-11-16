using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions.Loggers;

namespace IT.Tangdao.Framework.Abstractions
{
    internal class TangdaoConfigLoader : ITangdaoConfigLoader
    {
        private static readonly ITangdaoLogger Logger = TangdaoLogger.Get(typeof(TangdaoConfigLoader));

        // 嵌入资源路径 = 默认命名空间 + 文件夹 + 文件名
        private const string ResourcePath = "IT.Tangdao.Framework.Settings.tangdao.json";

        public TangdaoConfig Load()
        {
            var asm = Assembly.GetExecutingAssembly();
            // Logger.Debug();
            var stream = asm.GetManifestResourceStream(ResourcePath)
                ?? throw new InvalidOperationException(
                    $"找不到嵌入资源 '{ResourcePath}'，实际资源：{string.Join(", ", asm.GetManifestResourceNames())}");
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<TangdaoConfig>(json);
        }
    }
}