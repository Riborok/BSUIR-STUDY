using System;

namespace lap1_ {
	public static class PortReader {
		private const int MinPort = 0;
		private const int MaxPort = ushort.MaxValue;

		public static int ReadPortStartPort() => ReadPort(MinPort);
		
		private static int ReadPort(int defaultValue) {
			while (true) {
				Console.WriteLine("Enter port");
				var line = Console.ReadLine();
				if (line == string.Empty)
					return defaultValue;
				if (int.TryParse(line, out int port) && port >= MinPort && port <= MaxPort)
					return port;
				Console.WriteLine("Invalid port");
			}
		}

		public static int ReadPortEndPort(int startPort) {
			while (true) {
				int endPort = ReadPort(MaxPort);
				if (endPort < startPort)
					Console.WriteLine($"The end port ({endPort}) cannot be less than the start port ({startPort}).");
				else 
					return endPort;
			}
		}
	}
}