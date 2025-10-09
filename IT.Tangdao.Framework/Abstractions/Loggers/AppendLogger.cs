using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT.Tangdao.Framework.Extensions;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    internal sealed class AppendLogger : ITangdaoLogger
    {
        private readonly string _filePath;

        public AppendLogger(string folder)
        {
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, $"{DateTime.Now:yyyyMMdd}.log");
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