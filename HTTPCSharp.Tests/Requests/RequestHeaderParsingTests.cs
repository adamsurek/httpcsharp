using System.Text;
using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Tests.Requests;

public class RequestHeaderParsingTests
{
	[Theory]
	[InlineData("GET / HTTP/1.1\r\nContent-Type: application/json\r\nUser-Agent: PostmanRuntime/7.38.0\r\nAccept: */*\r\nPostman-Token: bf3fb80f-42f1-4755-90f8-308952d2eb91\r\nHost: 127.0.0.1:42069\r\nAccept-Encoding: gzip, deflate, br\r\nConnection: keep-alive\r\nContent-Length: 3\r\n\r\n123", 8)]
	public void RequestLineParsing_ReturnsExpectedHeaderCount(string request, int expectedHeaderCount)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(request);
		RequestParser parser = new(bytes);
		List<RequestHeader> headers = parser.Parse().RequestHeaders;

		Assert.Equal(expectedHeaderCount, headers.Count);
	}
}