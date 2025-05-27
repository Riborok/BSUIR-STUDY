using System;
using System.Net;

namespace lap1_ {
	public static class IPReader {
		public static IPAddress ReadIpAddress() {
			while (true) {
				Console.WriteLine("Enter IP address");
				if (IPAddress.TryParse(Console.ReadLine() ?? "", out IPAddress ipAddress))
					return ipAddress;
				Console.WriteLine("Invalid IP address");
			}
		}
	}
}