using System.Net.Sockets;

namespace OpenPorts;

public static class PortScanner {
    public static void ScanPorts(string ip, int startPort, int endPort, Dictionary<int, string> portServiceMap) {
        for (int port = startPort; port <= endPort; port++) {
            string portType = IsOpenPort(ip, port) ? "open" : "closed";
            string service = portServiceMap.GetValueOrDefault(port) ?? "none";
            Console.WriteLine($"Port {port} is {portType}! Service: {service}");
        }
    }

    private static bool IsOpenPort(string ip, int port) {
        try {
            var client = new TcpClient(ip, port);
            client.Dispose();
            return true;
        }
        catch (SocketException e) {
            return false;
        }
    }
}