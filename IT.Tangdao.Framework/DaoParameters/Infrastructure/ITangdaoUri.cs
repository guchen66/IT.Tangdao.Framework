using IT.Tangdao.Framework.DaoEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoParameters.Infrastructure
{
    public interface ITangdaoUri
    {
        Uri Uri { get; }
        NetConnectionType ConnectionType { get; }
        string Host { get; }
        int Port { get; }
        string ComPort { get; }
        int BaudRate { get; }
        bool IsValid { get; }
    }
}