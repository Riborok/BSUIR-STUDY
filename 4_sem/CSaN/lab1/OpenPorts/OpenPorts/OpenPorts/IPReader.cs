using System.Net;

namespace OpenPorts;

public static class IpReader {
	public static IPAddress ReadIpAddress() {
		while (true) {
			Console.WriteLine("Enter IP address");
			if (IPAddress.TryParse(Console.ReadLine() ?? string.Empty, out var ipAddress))
				return ipAddress;
			Console.WriteLine("Invalid IP address");
		}
	}
}