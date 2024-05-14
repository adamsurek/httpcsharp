using System.Text;
using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Tests.Requests;

public class RequestLineParsingTests
{
	[Theory]
	[InlineData("OPTIONS / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Options)]
	[InlineData("GET / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Get)]
	[InlineData("HEAD / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Head)]
	[InlineData("POST / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Post)]
	[InlineData("PUT / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Put)]
	[InlineData("DELETE / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Delete)]
	[InlineData("TRACE / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Trace)]
	[InlineData("CONNECT / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", RequestMethodEnum.Connect)]
	public void RequestLineParsing_ReturnsCorrectHTTPMethod(string requestLine, RequestMethodEnum expectedMethod)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		RequestParser parser = new(bytes);

		Assert.Equal(expectedMethod, parser.Parse().RequestLine.Method);
	}
	
	[Theory]
	[InlineData("GET / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", "/")]
	[InlineData("GET /test HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", "/test")]
	[InlineData("GET /test/test2/test/3 HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", "/test/test2/test/3")]
	[InlineData("POST /test123/ HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", "/test123/")]
	public void RequestLineParsing_ReturnsCorrectUri(string requestLine, string expectedUri)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		RequestParser parser = new(bytes);
		RequestLine parsed = parser.Parse().RequestLine;

		Assert.Equal(expectedUri, parsed.RequestUri.ToString());
	}
	
	[Theory]
	[InlineData("GET / HTTP/1.0\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", 1, 0)]
	[InlineData("GET / HTTP/1.1\r\nComment: This is a test\r\nUser-Agent: test\r\nAccept-Encoding: gzip, deflate, br\r\n\r\n\r\n", 1, 1)]
	public void RequestLineParsing_ReturnsCorrectHttpVersion(string requestLine, int majorVersion, int minorVersion)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(requestLine);
		RequestParser parser = new(bytes);
		RequestLine parsed = parser.Parse().RequestLine;

		Assert.Multiple(
			() => Assert.Equal(majorVersion, parsed.HttpVersion.MajorVersion),
			() => Assert.Equal(minorVersion, parsed.HttpVersion.MinorVersion)
		);
	}
}