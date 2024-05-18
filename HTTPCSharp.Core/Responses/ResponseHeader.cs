using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Core.Responses;

public class ResponseHeader
{
	public readonly HeaderField HeaderField;
	public readonly string HeaderValue;

	public ResponseHeader(HeaderFieldTypeEnum fieldType, string headerValue)
	{
		HeaderField = new HeaderField(fieldType);
		HeaderValue = headerValue;
	}

	public override string ToString()
	{
		return $"{HeaderField.Name}: {HeaderValue}\r\n";
	}
}
