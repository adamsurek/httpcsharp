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
	private ServerConfig _config = ConfigurationManager.Instance.ServerConfig;

	public async Task Listen()
	{
		RequestEvaluator evaluator = new(_config);
		
		IPEndPoint endPoint = new (ServerIp, ServerPort);
		_listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		_listener.Bind(endPoint);
		_listener.Listen();
		
		Console.WriteLine($"Listening at {endPoint}");

		while (true)
		{
			Socket client = await _listener.AcceptAsync();
			_ = Task.Run(() => HandleRequestAsync(client, evaluator));
		}
	}

	private async Task HandleRequestAsync(Socket handler, RequestEvaluator evaluator)
	{
			byte[] buffer = new byte[4096];
			
			await handler.ReceiveAsync(buffer, SocketFlags.None);
			
			/* REQUEST DATA */
			RequestParser parser = new(buffer);
			Request request = parser.Parse();
			/* END REQUEST DATA */
			
			/* RESPONSE DATA */
			Response response = await evaluator.EvaluateRequestAsync(request);
			byte[] encodedResponse = Encoding.ASCII.GetBytes(response.ToString());
			byte[]? body = response.ResponseBody;

			// Send status line and headers
			await handler.SendAsync(encodedResponse, 0);
		
			if (body is not null)
			{
				await handler.SendAsync(body, 0);
			}
			/* END RESPONSE DATA */
			
			handler.Shutdown(SocketShutdown.Both);	
			handler.Close();
	}
}