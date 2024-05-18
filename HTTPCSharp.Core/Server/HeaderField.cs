namespace HTTPCSharp.Core.Requests;



private Dictionary<HeaderFieldEnum, string> _headerFieldTranslation = new()
{
	{ HeaderFieldEnum.Accept, "Accept" },
	{ HeaderFieldEnum.AcceptCharset, "Accept-Charset" },
	{ HeaderFieldEnum., "Accept" },
	{ HeaderFieldEnum.Accept, "Accept" },
	{ HeaderFieldEnum.Accept, "Accept" },
	{ HeaderFieldEnum.Accept, "Accept" },
};
public enum HeaderFieldEnum
{
	Accept,
	AcceptCharset,
	AcceptEncoding,
	AcceptLanguage,
	Authorization,
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