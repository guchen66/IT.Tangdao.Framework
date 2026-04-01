using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Configurations;

namespace IT.Tangdao.Framework.Abstractions
{
    public class TangdaoRequest : ITangdaoRequest
    {
        private readonly AutoConfigurationManager _configManager;

        public TangdaoRequest()
        {
            _configManager = AutoConfigurationManager.Instance;
        }

        public T GetConfig<T>(string filePath = null) where T : class, new()
        {
            //return _configManager.Load<T>(filePath ?? GetDefaultPath<T>());

            return default(T);
        }

        public T GetConfig<T>(string filePath, Action<T> onLoaded) where T : class, new()
        {
            var config = GetConfig<T>(filePath);
            onLoaded?.Invoke(config);
            return config;
        }

        public void ReloadConfig<T>(string filePath = null)
        {
            //_configManager.Reload<T>(filePath ?? GetDefaultPath<T>());
        }

        private string GetDefaultPath<T>()
        {
            // 默认路径：程序目录/Configs/类名.json
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", $"{typeof(T).Name}.json");
        }
    }
}