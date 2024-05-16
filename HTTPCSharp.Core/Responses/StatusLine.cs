namespace HTTPCSharp.Core.Responses;

public class StatusLine
{
	public const string HttpVersion = "HTTP/1.1";
	public readonly StatusCodesEnum StatusCode;
	public readonly string ReasonPhrase;

	public StatusLine(StatusCodesEnum statusCode, string reasonPhrase)
	{
		StatusCode = statusCode;
		ReasonPhrase = reasonPhrase;
	}

	public override string ToString()
	{
		return $"{HttpVersion} {(int)StatusCode} {ReasonPhrase}\r\n";
	}
}