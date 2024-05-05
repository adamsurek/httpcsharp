namespace HTTPCSharp.Core.Requests;

public class Request
{
	public readonly RequestLine RequestLine;

	public Request(RequestLine requestLine)
	{
		RequestLine = requestLine;
	}
}