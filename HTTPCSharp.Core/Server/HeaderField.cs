namespace HTTPCSharp.Core.Requests;
	
public enum HeaderFieldTypeEnum
{
	Accept,
	AcceptCharset,
	AcceptEncoding,
	AcceptLanguage,
	AcceptRanges,
	Allow,
	ContentEncoding,
	ContentLength,
	ContentType,
	Date,
	Host,
	IfMatch,
	IfModifiedSince,
	IfRange,
	IfUnmodifiedSince,
	Range,
	Server,
	UserAgent
}

public class HeaderField
{
	private readonly Dictionary<HeaderFieldTypeEnum, string> _headerFieldTranslation = new()
	{
		{ HeaderFieldTypeEnum.Accept, "Accept" },
		{ HeaderFieldTypeEnum.AcceptCharset, "Accept-Charset" },
		{ HeaderFieldTypeEnum.AcceptEncoding, "Accept-Encoding" },
		{ HeaderFieldTypeEnum.AcceptLanguage, "Accept-Language" },
		{ HeaderFieldTypeEnum.AcceptRanges, "Accept-Ranges" },
		{ HeaderFieldTypeEnum.Allow, "Allow" },
		{ HeaderFieldTypeEnum.ContentEncoding, "Content-Encoding" },
		{ HeaderFieldTypeEnum.ContentLength, "Content-Length" },
		{ HeaderFieldTypeEnum.ContentType, "Content-Type" },
		{ HeaderFieldTypeEnum.Date, "Date" },
		{ HeaderFieldTypeEnum.Host, "Host" },
		{ HeaderFieldTypeEnum.IfMatch, "If-Match" },
		{ HeaderFieldTypeEnum.IfModifiedSince, "If-Modified-Since" },
		{ HeaderFieldTypeEnum.IfRange, "If-Range" },
		{ HeaderFieldTypeEnum.IfUnmodifiedSince, "If-Unmodified-Since" },
		{ HeaderFieldTypeEnum.Range, "Range" },
		{ HeaderFieldTypeEnum.Server, "Server" },
		{ HeaderFieldTypeEnum.UserAgent, "User-Agent" },
	};

	public readonly HeaderFieldTypeEnum Type;
	public readonly string Name;

	public HeaderField(HeaderFieldTypeEnum headerFieldType)
	{
		Type = headerFieldType;
		Name = _headerFieldTranslation[headerFieldType];
	}
}
