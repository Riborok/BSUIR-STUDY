using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
	internal static class Program {
		public static void Main(string[] args) 
		{
			using (var server = new HttpServer("http://localhost:8081/"))
			{
				var localServer = server;
				var processingTask = Task.Run(() =>
				{
					while (!IsPressed(ConsoleKey.Escape))
						Thread.Sleep(42);
					localServer.Stop();
				});

				server.Start();
				processingTask.Wait();
			}
		}

		private static bool IsPressed(ConsoleKey ck) => Console.KeyAvailable && Console.ReadKey(true).Key == ck;
	}
}
