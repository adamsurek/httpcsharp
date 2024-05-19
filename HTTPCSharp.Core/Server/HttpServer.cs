using System.Net;
using System.Net.Sockets;
using System.Text;
using HTTPCSharp.Core.Requests;
using HTTPCSharp.Core.Responses;

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
			byte[] buffer = new byte[4096];
			string message = "";
			
			Console.WriteLine("Awaiting connection...");
			
			Socket client = _listener.Accept();
			Console.WriteLine($"Client Connected: {client.Connected}");

			int received = client.Receive(buffer, SocketFlags.None);
			message += Encoding.UTF8.GetString(buffer, 0, received);
			Console.WriteLine($"Received >> \n{message}");
			
			/* REQUEST DATA */
			RequestParser parser = new(buffer);
			Request request = parser.Parse();
			RequestLine requestLine = request.RequestLine;
			
			Console.WriteLine($"Request Method: {requestLine.Method}\tURI: path - {requestLine.RequestUri.Path}, query - {requestLine.RequestUri.Query}\tVersion: {requestLine.HttpVersion}");
			
			foreach (RequestHeader header in request.RequestHeaders)
			{
				Console.WriteLine($"Header Type: '{header.HeaderType}' - Header Value: '{header.HeaderValue}'");
			}
			
			Console.WriteLine($"Body: '{request.RequestBody}'");
			/* END REQUEST DATA */
			
			/* RESPONSE DATA */
			Response response = RequestEvaluator.EvaluateRequest(request);
			
			// Console.WriteLine($"\r\nRESPONSE: {response}");
			
			byte[] encodedResponse = Encoding.ASCII.GetBytes(response.ToString());
			
			client.Send(encodedResponse);
			/* END RESPONSE DATA */
			
			client.Shutdown(SocketShutdown.Both);	
			client.Close();
		}
	}
}