using System;
using System.Net;
using System.Net.Sockets;

namespace lap1_ {
	internal static class Program {
		public static void Main(string[] args) {
			IPAddress ip = IPReader.ReadIpAddress();
			int startPort = PortReader.ReadPortStartPort();
			int endPort = PortReader.ReadPortEndPort(startPort);

			PortScanner.ScanPorts(ip.ToString(), startPort, endPort);
			Console.ReadLine();
		}
	}
}