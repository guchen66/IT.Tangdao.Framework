using IT.Tangdao.Framework.DaoDtos.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoEnums
{
    public enum LogLevel
    {
        Off,
        Critical,
        Error,
        Warning,
        Information,
        Trace
    }
    public class Logger
    {
        public LogLevel EnabledLevel { get; set; } = LogLevel.Error;

        public string LogMessage(LogLevel level, string msg)
        {
            if (EnabledLevel < level)
            {
                return msg;
            }
            else
            {
                return "未知姓名";
            }
        }

        public void LogMessage2(LogLevel level, DaoErrorHandler builder)
        {
            if (EnabledLevel < level) return;
            Console.WriteLine(builder.GetFormattedText());
        }
    }
}
