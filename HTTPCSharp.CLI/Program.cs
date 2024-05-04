using HTTPCSharp.Core.Server;

namespace HTTPCSharp.CLI;

class Program
{
	static void Main(string[] args)
	{
		HttpServer server = new();
		Console.WriteLine($"Server Initialized: {server.ServerIp}\tPort: {server.ServerPort}");
		
		server.Listen();
	}
}