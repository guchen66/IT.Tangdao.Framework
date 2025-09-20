using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Abstractions;
using IT.Tangdao.Framework.Parameters.Infrastructure;

namespace IT.Tangdao.Framework.Extensions
{
    /// <summary>
    /// 日志扩展类
    /// </summary>
    public static class TangdaoLoggerExtension
    {
        /// <summary>
        /// 把日志写到应用层指定的目录（或默认桌面）。
        /// category 为空时直接落在根目录，否则作为子文件夹。
        /// 程序启动LogPathConfig.SetRoot可自定义Log目录
        /// </summary>
        public static void WriteLocal(this IDaoLogger logger, string message, string category = null)
        {
            if (logger == null) return;

            try
            {
                var root = LogPathConfig.Root;
                var dir = string.IsNullOrEmpty(category) ? root : Path.Combine(root, category);
                Directory.CreateDirectory(dir);

                var fileName = $"{DateTime.Now:yyyyMMdd}_local.log";
                var filePath = Path.Combine(dir, fileName);

                // 这里用到了 logger，于是扩展方法“名副其实”
                var line = $"{DateTime.Now:F}  {logger.GetType().FullName}  {message}{Environment.NewLine}";
                File.AppendAllText(filePath, line);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WriteLocal failed: {ex}");
            }
        }
    }
}