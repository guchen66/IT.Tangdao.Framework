using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Abstractions.Loggers
{
    public interface ITangdaoLogger
    {
        void Fatal(string message, Exception e = null);

        void Error(string message, Exception e = null);

        void Warning(string message, Exception e = null);

        void Info(string message, Exception e = null);

        void Debug(string message, Exception e = null);
    }
}