namespace HTTPCSharp.Core.Requests;

public class Request
{
	public readonly RequestLine RequestLine;
	public readonly List<RequestHeader> RequestHeaders;
	public string? RequestBody;

	public Request(RequestLine requestLine, List<RequestHeader> requestHeaders, string? requestBody)
	{
		RequestLine = requestLine;
		RequestHeaders = requestHeaders;
		RequestBody = requestBody;
	}
}