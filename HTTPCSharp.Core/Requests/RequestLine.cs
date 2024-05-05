namespace HTTPCSharp.Core.Requests;

public class RequestLine
{
	public readonly RequestMethodEnum Method;
	public readonly string RequestUri;
	public readonly HttpVersion HttpVersion;

	public RequestLine(RequestMethodEnum method, string requestUri, HttpVersion httpVersion)
	{
		Method = method;
		RequestUri = requestUri;
		HttpVersion = httpVersion;
	}
}