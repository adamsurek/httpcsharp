using System.Net;
using System.Net.Sockets;
using System.Text;
using HTTPCSharp.Core.Requests;

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

			int received = client.Receive(buffer, SocketFlags.None);
			message += Encoding.UTF8.GetString(buffer, 0, received);
			Console.WriteLine($"Received >> \n{message}");

			// Lexer lexer = new(buffer);
			// RequestParser requestParser = new(lexer);
			// var requestLine = requestParser.Parse().RequestLine;

			RequestParser parser = new(buffer);
			RequestLine requestLine = parser.Parse().RequestLine;
			
			Console.WriteLine($"Request Method: {requestLine.Method}\tURI: path - {requestLine.RequestUri.Path}, query - {requestLine.RequestUri.Query}\tVersion: {requestLine.HttpVersion}");
			
			string responseString = $"HTTP/1.1 200 OK \r\nComment: Test\r\n\r\n{message}\r\n";
			byte[] response = Encoding.UTF8.GetBytes(responseString);
			client.Send(response);
			
			client.Shutdown(SocketShutdown.Both);	
			client.Close();
		}
	}
}