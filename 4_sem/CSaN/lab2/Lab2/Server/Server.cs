using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
	public class Server : IDisposable
	{
		private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly List<Socket> _clients = new List<Socket>();
		private readonly byte[] _buffer = new byte[1024];

		public void Start(IPAddress ipAddress, int port)
		{
			while (port <= ushort.MaxValue)
			{
				TcpListener listener = new TcpListener(ipAddress, port);
				try
				{
					listener.Start();
					listener.Stop();
					break;
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"Socket Error: {ex.SocketErrorCode} with port {port}");
					port++;
				}
			}
			if (port > ushort.MaxValue)
				throw new SocketException((int)SocketError.AddressAlreadyInUse);

			_socket.Bind(new IPEndPoint(ipAddress, port));
			_socket.Listen(5);
			Console.WriteLine($"The server is running!!! IP: {ipAddress} Port: {port}");
			
			_socket.BeginAccept(AcceptCallback, null);
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			Socket clientSocket = _socket.EndAccept(ar);
			_clients.Add(clientSocket);
			Console.WriteLine("The client is connected");

			clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, clientSocket);
			_socket.BeginAccept(AcceptCallback, null);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			Socket clientSocket = (Socket)ar.AsyncState;
			
			int bufferSize = clientSocket.EndReceive(ar, out SocketError errorCode);
			if (errorCode == SocketError.Success)
			{
				var ipEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
				var clientIp = Encoding.UTF8.GetBytes($"{ipEndPoint.Address}:{ipEndPoint.Port}   ");
				byte[] packet = new byte[bufferSize + clientIp.Length];
				Array.Copy(clientIp, packet, clientIp.Length); 
				Array.Copy(_buffer, 0, packet, clientIp.Length, bufferSize); 
				
				BroadcastMessage(packet);
				clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, clientSocket);
			}
			else
			{
				Console.WriteLine("Client is disconnected");
				_clients.Remove(clientSocket);
				clientSocket.Close();
			}
		}

		private void BroadcastMessage(byte[] message)
		{
			foreach (var client in _clients)
				client.BeginSend(message, 0, message.Length, SocketFlags.None, SendCallback, client);
		}

		private static void SendCallback(IAsyncResult ar)
		{
			Socket clientSocket = (Socket)ar.AsyncState;
			clientSocket.EndSend(ar);
		}

		public void Dispose()
		{
			foreach (var client in _clients)
				client.Close();
			_socket.Dispose();
		}
	}
}