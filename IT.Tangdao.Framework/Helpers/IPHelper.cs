using IT.Tangdao.Framework.DaoException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Helpers
{
    public class IPHelper
    {
        /// <summary>
        /// 打印本地所有IP，包括回路
        /// </summary>
        public static string GetLocalIPByDns()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresss = entry.AddressList;
            foreach (IPAddress addr in addresss)
            {
                return addr.ToString();
            }

            throw new ContainerErrorException($"未找到IP");
        }

        /// <summary>
        /// 不包括主机，返回IP4
        /// </summary>
        public static string GetLocalIP2()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresss = entry.AddressList;
            foreach (IPAddress ip in addresss)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new ContainerErrorException($"未找到IP");
        }

        /// <summary>
        /// 打印以太网IP4
        /// </summary>
        public static string GetLocalIPByLinq()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new ContainerErrorException($"No Network Available");
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            var ippaddress = host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ippaddress?.ToString();
        }

        /// <summary>
        /// 打印本地所有IP，包括回路
        /// </summary>
        public static string GetLocalIP(NetworkInterfaceType _type)
        {
            string output = "127.0.0.1";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        public static string FindIPByName(string interfaceName)
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in networkInterfaces)
            {
                // 检查网络接口的名称或描述是否与提供的名称匹配
                if (ni.Name.Equals(interfaceName, StringComparison.OrdinalIgnoreCase) ||
                    (ni.Description != null && ni.Description.Equals(interfaceName, StringComparison.OrdinalIgnoreCase)))
                {
                    IPInterfaceProperties ipProperties = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in ipProperties.UnicastAddresses)
                    {
                        Console.WriteLine($"Interface: {ni.Name} ({ni.Description}) - IP: {ip.Address}");
                        return ip.Address.ToString();// 假设只查找第一个匹配的接口
                    }
                }
            }
            throw new ContainerErrorException($"No network interface found with the name or description: {interfaceName}");
        }

        /// <summary>
        /// 打印本地所有IP，包括回路
        /// </summary>
        public static string GetLocalIPBySocket()
        {
            string ip = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ip = endPoint.Address.ToString();
            }
            return ip.ToString();
        }

        /// <summary>
        /// Uri转换未IP
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static IPAddress UriToIPAddress(Uri uri)
        {
            if (uri.HostNameType == UriHostNameType.IPv4 ||
                uri.HostNameType == UriHostNameType.IPv6)
                return IPAddress.Parse(uri.Host);          // 192.168.1.100

            // 如果是Dns名，先解析
            return Dns.GetHostAddresses(uri.Host)[0];      // 返回第一个地址
        }

        /// <summary>
        /// IP转Uri
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static Uri IPAddressToUri(IPAddress ip, int port, string scheme = "tcp")
        {
            // 0.0.0.0 特殊处理：监听地址写成 0.0.0.0，但 Uri 里建议写 *
            string host = ip.Equals(IPAddress.Any) ? "*" : ip.ToString();
            return new Uri($"{scheme}://{host}:{port}");
        }
    }
}