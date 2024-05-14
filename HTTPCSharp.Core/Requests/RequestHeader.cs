namespace HTTPCSharp.Core.Requests;

public class RequestHeader
{
	public readonly string HeaderType;
	public readonly string HeaderValue;

	public RequestHeader(string headerType, string headerValue)
	{
		HeaderType = headerType;
		HeaderValue = headerValue;
	}
}