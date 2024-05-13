namespace HTTPCSharp.Core.Requests;

public class RequestUri
{
	public readonly string Scheme;
	public readonly string Host;
	public readonly int Port;
	public readonly string Path;
	public readonly string Query;
	public readonly string Fragment;

	public RequestUri(string scheme, string host, int port, string path, string query, string fragment)
	{
		
	}

	public RequestUri()
	{
		
	}

}