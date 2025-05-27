using System.Net;

namespace OpenPorts;

internal static class Program {
    public static void Main() {
        var mapTask = PortServiceMapper.GetPortServiceDictionary();
        
        IPAddress ip = IpReader.ReadIpAddress();
        int startPort = PortReader.ReadPortStartPort();
        int endPort = PortReader.ReadPortEndPort(startPort);

        mapTask.Wait();
        PortScanner.ScanPorts(ip.ToString(), startPort, endPort, mapTask.Result);
        Console.ReadLine();
    }
}