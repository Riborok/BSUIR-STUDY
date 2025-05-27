using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Lab1
{
    internal static class Networks
    {
        public static List<IpMaskName> GetIpAndMask()
        {
            var arr = new List<IpMaskName>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProperties = networkInterface.GetIPProperties();
                    foreach (var ipAddress in ipProperties.UnicastAddresses)
                    {
                        if (!IPAddress.IsLoopback(ipAddress.Address) && ipAddress.Address.AddressFamily ==
                            System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            var ip = ipAddress.Address.ToString();
                            var mask = ipAddress.IPv4Mask.ToString();
                            arr.Add(new IpMaskName { Ip = ip, Mask = mask, Name = networkInterface.Name });
                        }
                    }
                }
            }
            return arr;
        }

        private static uint Ip2Long(string ip)
        {
            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private static string Long2Ip(uint ipInt)
        {
            byte[] bytes = BitConverter.GetBytes(ipInt);
            Array.Reverse(bytes);
            return new IPAddress(bytes).ToString();
        }
        
        public static IEnumerable<string> GetIps(string strMask, string strIp)
        {
            var arr = new List<string>();
            uint maskInt = Ip2Long(strMask);
            uint ipInt = Ip2Long(strIp);

            uint broadcastInt = ipInt | ~maskInt;
            uint startIp = ipInt & maskInt;

            for (uint ip = startIp; ip <= broadcastInt; ip++)
                arr.Add(Long2Ip(ip));

            return arr;
        }
        
        public class IpMaskName
        {
            public string Ip { get; set; }
            public string Mask { get; set; }
            public string Name { get; set; }
        }
    }
}