﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPCSharp.Core.Server;

public class HttpServer
{
	public readonly IPAddress ServerIp = IPAddress.Loopback;
	public readonly int ServerPort = 42069;
	private Socket? _listener;
	
	public void Listen()
	{
		IPEndPoint endPoint = new (ServerIp, ServerPort);
		_listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		_listener.Bind(endPoint);
		_listener.Listen(10);
		
		Console.WriteLine($"Listening at {endPoint}");

		while (true)
		{
			byte[] buffer = new byte[1024];
			string message = "";
			
			Console.WriteLine("Awaiting connection...");
			
			Socket client = _listener.Accept();
			Console.WriteLine($"Client Connected: {client.Connected}");

			int received = client.Receive(buffer);
			message += Encoding.UTF8.GetString(buffer, 0, received);
			Console.WriteLine($"Received >> \n{message}");
			
			string responseString = $"HTTP/1.1 \r\nComment: Test\r\n\r\n{message}\r\n";
			byte[] response = Encoding.UTF8.GetBytes(responseString);
			client.Send(response);
			
			client.Shutdown(SocketShutdown.Both);	
			client.Close();
		}
	}
}