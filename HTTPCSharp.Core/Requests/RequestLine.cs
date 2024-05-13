namespace HTTPCSharp.Core.Requests;

public class RequestLine
{
	public readonly RequestMethodEnum Method;
	public readonly RequestUri RequestUri;
	public readonly HttpVersion HttpVersion;

	public RequestLine(RequestMethodEnum method, RequestUri requestUri, HttpVersion httpVersion)
	{
		Method = method;
		RequestUri = requestUri;
		HttpVersion = httpVersion;
	}
}