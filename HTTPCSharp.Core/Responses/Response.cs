using System.Text;

namespace HTTPCSharp.Core.Responses;

public class Response
{
	public StatusLine StatusLine;
	public List<ResponseHeader> ResponseHeaders;
	public string? ResponseBody;

	public Response(StatusLine statusLine, List<ResponseHeader> responseHeaders, string? responseBody = null)
	{
		StatusLine = statusLine;
		ResponseHeaders = responseHeaders;
		ResponseBody = responseBody;
	}

	public override string ToString()
	{
		StringBuilder builder = new();

		builder.Append(StatusLine);

		foreach (ResponseHeader header in ResponseHeaders)
		{
			builder.Append(header);
		}

		builder.Append("\r\n");
		builder.Append(ResponseBody);

		return builder.ToString();
	}
}
