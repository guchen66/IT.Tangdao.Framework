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
        public static void GetLocalIPByDns()
        {
            IPHostEntry entry=Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresss=entry.AddressList;
            foreach (IPAddress addr in addresss)
            {
                Console.WriteLine(addr);
            }
        }

        /// <summary>
        /// 不包括主机，返回IP4
        /// </summary>
        public static void GetLocalIP2()
        {
            IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addresss = entry.AddressList;
            foreach (IPAddress ip in addresss)
            {
                if (ip.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Console.WriteLine(ip.ToString());
                }
               
            }
        }

        /// <summary>
        /// 打印以太网IP4
        /// </summary>
        public static void GetLocalIPByLinq()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Console.WriteLine("No Network Available");
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            var ippaddress = host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            Console.WriteLine(ippaddress);
        }

        /// <summary>
        /// 打印本地所有IP，包括回路
        /// </summary>
        public static void GetLocalIP(NetworkInterfaceType _type)
        {
            string output = "";
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
            Console.WriteLine("IP Address = " + output);
        }

        public static void FindIPByName(string interfaceName)
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
                    }
                    return; // 假设只查找第一个匹配的接口
                }
            }
            Console.WriteLine($"No network interface found with the name or description: {interfaceName}");
        }

        /// <summary>
        /// 打印本地所有IP，包括回路
        /// </summary>
        public static void GetLocalIPBySocket()
        {
            string ip = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ip = endPoint.Address.ToString();
            }
            Console.WriteLine(ip);
        }
    }
}
