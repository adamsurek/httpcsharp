using HTTPCSharp.Core.Server;

namespace HTTPCSharp.CLI;

class Program
{
	static async Task Main(string[] args)
	{
		HttpServer server = new();
		Console.WriteLine($"Server Initialized: {server.ServerIp}\tPort: {server.ServerPort}");
		
		await server.Listen();
	}
}