namespace OpenPorts;

public static class PortReader {
	private const int MinPort = 0;
	private const int MaxPort = ushort.MaxValue;

	public static int ReadPortStartPort() {
		return ReadPort(MinPort, "start");
	}

	public static int ReadPortEndPort(int startPort) {
		while (true) {
			int endPort = ReadPort(MaxPort, "end");
			if (endPort < startPort)
				Console.WriteLine($"The end port ({endPort}) cannot be less than the start port ({startPort}).");
			else 
				return endPort;
		}
	}
	
	private static int ReadPort(int defaultValue, string type) {
		while (true) {
			Console.WriteLine($"Enter {type} port");
			var line = Console.ReadLine();
			if (line == string.Empty)
				return defaultValue;
			if (int.TryParse(line, out int port) && port >= MinPort && port <= MaxPort)
				return port;
			Console.WriteLine("Invalid port");
		}
	}
}