using System;
using System.Net.Sockets;

namespace lap1_
{
    public class PortScanner {
        public static void ScanPorts(string ip, int startPort, int endPort) {
            for (int port = startPort; port <= endPort; port++) {
                if (IsOpenPort(ip, port))
                    Console.WriteLine($"Port {port} is open!");
            }
        }

        private static bool IsOpenPort(string ip, int port) {
            try {
                TcpClient client = new TcpClient(ip, port);
                client.Dispose();
                return true;
            }
            catch (SocketException e) {
                return false;
            }
        }
    }
}