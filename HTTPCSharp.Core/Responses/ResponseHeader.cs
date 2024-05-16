namespace HTTPCSharp.Core.Responses;

public class ResponseHeader
{
	public readonly string HeaderType;
	public readonly string HeaderValue;

	public ResponseHeader(string headerType, string headerValue)
	{
		HeaderType = headerType;
		HeaderValue = headerValue;
	}

	public override string ToString()
	{
		return $"{HeaderType}: {HeaderValue}\r\n";
	}
}
