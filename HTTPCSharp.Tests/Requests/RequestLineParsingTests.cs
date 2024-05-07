using System.Text;
using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Tests.Requests;

public class RequestLineParsingTests
{
	[Theory]
	[InlineData("OPTIONS / HTTP/1.1", RequestMethodEnum.Options)]
	[InlineData("GET / HTTP/1.1", RequestMethodEnum.Get)]
	[InlineData("HEAD / HTTP/1.1", RequestMethodEnum.Head)]
	[InlineData("POST / HTTP/1.1", RequestMethodEnum.Post)]
	[InlineData("PUT / HTTP/1.1", RequestMethodEnum.Put)]
	[InlineData("DELETE / HTTP/1.1", RequestMethodEnum.Delete)]
	[InlineData("TRACE / HTTP/1.1", RequestMethodEnum.Trace)]
	[InlineData("CONNECT / HTTP/1.1", RequestMethodEnum.Connect)]
	public void RequestLineParsing_ReturnsCorrectHTTPMethod(string requestLine, RequestMethodEnum expectedMethod)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		Lexer lexer = new(bytes);
		RequestParser parser = new(lexer);

		Assert.Equal(expectedMethod, parser.Parse().RequestLine.Method);
	}
	
	[Theory]
	[InlineData("GET / HTTP/1.1", "/")]
	[InlineData("GET /test HTTP/1.1", "/test")]
	public void RequestLineParsing_ReturnsCorrectUri(string requestLine, string expectedUri)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		Lexer lexer = new(bytes);
		RequestParser parser = new(lexer);

		Assert.Equal(expectedUri, parser.Parse().RequestLine.RequestUri);
	}
	
	[Theory]
	[InlineData("GET / HTTP/1.0", 1, 0)]
	[InlineData("GET / HTTP/1.1", 1, 1)]
	public void RequestLineParsing_ReturnsCorrectHttpVersion(string requestLine, int majorVersion, int minorVersion)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		Lexer lexer = new(bytes);
		RequestParser parser = new(lexer);
		RequestLine parsed = parser.Parse().RequestLine;

		Assert.Multiple(
			() => Assert.Equal(majorVersion, parsed.HttpVersion.MajorVersion),
			() => Assert.Equal(minorVersion, parsed.HttpVersion.MinorVersion)
		);
	}
}