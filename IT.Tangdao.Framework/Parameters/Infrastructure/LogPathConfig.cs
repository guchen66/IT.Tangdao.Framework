using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Parameters.Infrastructure
{
    /// <summary>
    /// 全局日志目录配置。由应用层在启动时赋值，若不赋值则落在桌面。
    /// </summary>
    public static class LogPathConfig
    {
        private static string _folder = null;          // 缓存
        private static readonly object _lock = new object();

        /// <summary>
        /// 应用层调用此方法一次性设置根目录。
        /// </summary>
        public static void SetRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be empty.");

            lock (_lock)
            {
                _ = Directory.CreateDirectory(path);   // 确保目录存在
                _folder = path;
            }
        }

        /// <summary>
        /// 内部/扩展方法统一拿路径。
        /// </summary>
        public static string Root
        {
            get
            {
                if (_folder == null)
                {
                    lock (_lock)
                    {
                        // 双重检查
                        if (_folder == null)
                            _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TangdaoLogs");
                        _ = Directory.CreateDirectory(_folder);
                    }
                }
                return _folder;
            }
        }
    }
}