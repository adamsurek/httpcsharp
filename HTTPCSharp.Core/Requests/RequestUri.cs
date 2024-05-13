using System.Text;

namespace HTTPCSharp.Core.Requests;

public class RequestUri
{
	public readonly string? Scheme;
	public readonly string? Host;
	public readonly int Port;
	public readonly string Path;
	public readonly string? Query;
	public readonly string? Fragment;

	public RequestUri(string? scheme, string? host, int port, string path, string? query, string? fragment)
	{
		Scheme = scheme;
		Host = host;
		Port = port;
		Path = path;
		Query = query;
		Fragment = fragment;
	}

	public override string ToString()
	{
		StringBuilder builder = new();
		
		if (Scheme is not null)
		{
			builder.Append($"{Scheme}://");
		}

		if (Host is not null)
		{
			builder.Append($"{Host}");
			if (Port != -1)
			{
				builder.Append($":{Port}");
			}
		}

		builder.Append(Path);

		if (Query is not null)
		{
			builder.Append($"?{Query}");
		}
		
		if (Fragment is not null)
		{
			builder.Append($"#{Fragment}");
		}

		return builder.ToString();
	}
}