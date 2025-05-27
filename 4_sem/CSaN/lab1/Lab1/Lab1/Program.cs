using System;
using System.Collections.Generic;

namespace Lab1
{
    internal static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter subnet mask:");
            string mask = Console.ReadLine();
            
            List<Networks.IpMaskName> ipsAndMasks;
            try
            {
                ipsAndMasks = Networks.GetIpAndMask();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            if (!string.IsNullOrEmpty(mask))
                foreach (var ipAndMask in ipsAndMasks)
                    ipAndMask.Mask = mask;

            Console.WriteLine($"Active networks count: {ipsAndMasks.Count}\n");

            int i = 1;
            foreach (var el in ipsAndMasks)
            {
                Console.WriteLine($"\nNetwork interface {i++}");
                Console.WriteLine($"Mask: {el.Mask}, Name: {el.Name}");
                
                TableCreator.DrawHeader();
                
                var ips = Networks.GetIps(el.Mask, el.Ip);
                foreach (var ip in ips)
                {
                    //if (Commands.Ping(ip))
                    {
                        try
                        {
                            var node = Commands.Arp(ip);
                            TableCreator.DrawRow(node);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
        }
    }
}