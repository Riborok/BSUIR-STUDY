using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{
	public class Client : IDisposable
	{
		private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly byte[] _buffer = new byte[1024];
		
		private readonly TextBox _textBox;

		public Client(TextBox textBox)
		{
			_textBox = textBox;
		}
		
		public void Connect(IPAddress ipAddress, int port)
		{
			_socket.BeginConnect(ipAddress, port, ConnectCallback, null);
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			_socket.EndConnect(ar);

			_socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			int bufferSize = _socket.EndReceive(ar);

			byte[] packet = new byte[bufferSize];
			Array.Copy(_buffer, packet, bufferSize);

			string receivedMessage = Encoding.UTF8.GetString(packet);
			_textBox.Invoke((MethodInvoker)(() => _textBox.Text += receivedMessage + Environment.NewLine));

			_socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
		}

		public void Send(string message)
		{
			byte[] packet = Encoding.UTF8.GetBytes(message);
			_socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendCallback, null);
		}

		private void SendCallback(IAsyncResult ar)
		{
			_socket.EndSend(ar);
		}

		public void Dispose() => _socket.Dispose();
	}
}