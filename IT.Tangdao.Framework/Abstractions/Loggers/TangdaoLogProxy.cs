using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    /// <summary>
    /// 代理：对外仍是 ITangdaoLogger，对内选策略
    /// </summary>
    internal sealed class TangdaoLogProxy : ITangdaoLogger
    {
        private readonly ITangdaoLogger _real;

        public TangdaoLogProxy(string folder, bool overwrite = false)
        {
            _real = overwrite ? (ITangdaoLogger)new OverwriteLogger(folder) : (ITangdaoLogger)new AppendLogger(folder);
        }

        public void Fatal(string message, Exception e = null) => _real.Fatal(message, e);

        public void Error(string message, Exception e = null) => _real.Error(message, e);

        public void Warning(string message, Exception e = null) => _real.Warning(message, e);

        public void Info(string message, Exception e = null) => _real.Info(message, e);

        public void Debug(string message, Exception e = null) => _real.Debug(message, e);
    }
}