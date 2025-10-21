using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Infrastructure
{
    public class TangdaoUri : ITangdaoUri
    {
        public Uri Uri { get; private set; }
        public NetConnectionType ConnectionType { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string ComPort { get; private set; }
        public int BaudRate { get; private set; }
        public bool IsValid { get; private set; }

        public TangdaoUri(string uriString)
        {
            ParseUri(uriString);
        }

        private void ParseUri(string uriString)
        {
            try
            {
                Uri = new Uri(uriString);

                switch (Uri.Scheme.ToLower())
                {
                    case "tcp":
                        ConnectionType = NetConnectionType.Tcp;
                        Host = Uri.Host;
                        Port = Uri.Port;
                        IsValid = true;
                        break;

                    case "udp":
                        ConnectionType = NetConnectionType.Udp;
                        Host = Uri.Host;
                        Port = Uri.Port;
                        IsValid = true;
                        break;

                    case "serial":
                        ConnectionType = NetConnectionType.Serial;
                        ComPort = Uri.Host;
                        BaudRate = ParseBaudRateFromQuery(Uri.Query);
                        IsValid = true;
                        break;

                    default:
                        IsValid = false;
                        break;
                }
            }
            catch
            {
                IsValid = false;
            }
        }

        public static TangdaoUri CreateTcp(string host, int port)
            => new TangdaoUri($"tcp://{host}:{port}");

        public static TangdaoUri CreateUdp(string host, int port)
            => new TangdaoUri($"udp://{host}:{port}");

        public static TangdaoUri CreateSerial(string comPort, int baudRate = 9600)
            => new TangdaoUri($"serial://{comPort}?baudrate={baudRate}");

        private static int ParseBaudRateFromQuery(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return 9600;

            // 移除开头的问号
            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            // 分割参数
            var parameters = queryString.Split('&');
            foreach (var param in parameters)
            {
                var keyValue = param.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Equals("baudrate", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(keyValue[1], out int baudRate))
                        return baudRate;
                }
            }

            return 9600; // 默认值
        }
    }
}