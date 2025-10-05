using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    internal class OverwriteLogger : ITangdaoLogger
    {
        private readonly string _filePath;

        public OverwriteLogger(string folder)
        {
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd}_dll.log");
            // 启动即清空
            File.WriteAllText(_filePath, string.Empty, Encoding.UTF8);
        }

        public void Debug(string message, Exception e = null)
        {
            this.Debug(message);
        }

        public void Error(string message, Exception e = null)
        {
            this.Error(message);
        }

        public void Fatal(string message, Exception e = null)
        {
            this.Fatal(message);
        }

        public void Info(string message, Exception e = null)
        {
            this.Info(message);
        }

        public void Warning(string message, Exception e = null)
        {
            this.Warning(message);
        }
    }
}