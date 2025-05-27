using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Lab1
{
    internal static class Commands
    {
        public static bool Ping(string ip)
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send(ip);
                    return reply != null && reply.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    return false;
                }
            }
        }
        
        public static Node Arp(string ip)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "arp",
                Arguments = "-a " + ip,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length >= 2)
                    {
                        string[] cols = lines[2].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (cols.Length >= 2)
                            return new Node { Ip = cols[0], Mac = cols[1], Name = cols[2] };
                    }
                }
                return new Node { Ip = ip, Mac = "unknown", Name = "unknown" };
            }
        }
    }
}