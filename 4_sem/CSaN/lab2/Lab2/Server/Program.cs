using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			Server server = new Server();

			try
			{
				server.Start(ReadIpAddress(), ReadPort());
			}
			catch (SocketException se)
			{
				Console.WriteLine(se);
				return;
			}
			
			Console.ReadLine();
			server.Dispose();
		}
		
		private static IPAddress ReadIpAddress()
		{
			while (true)
			{
				Console.WriteLine("Enter IP address");

				var line = Console.ReadLine();
				if (string.IsNullOrEmpty(line)) 
					return IPAddress.Parse("127.0.0.1");
				if (IPAddress.TryParse(line, out IPAddress ipAddress))
					return ipAddress;

				Console.WriteLine("Invalid IP address");
			}
		}
		
		private static int ReadPort()
		{
			while (true)
			{
				Console.WriteLine("Enter port");
				var line = Console.ReadLine();
				if (line == "") 
					return 8081;
				if (int.TryParse(line, out int port) && port >= 0 && port <= ushort.MaxValue)
					return port;
				
				Console.WriteLine("Invalid port");
			}
		}
	}
}