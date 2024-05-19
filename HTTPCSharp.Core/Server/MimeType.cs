namespace HTTPCSharp.Core.Server;

public enum MimeTypeEnum
{
	TextCsv,
	TextHtml,
	TextPlain,
	TextJavascript,
	ApplicationJson,
	ImageJpeg,
	ImagePng,
	ApplicationOctetStream
}

public class MimeType
{
	private readonly Dictionary<string, MimeTypeEnum> _mimeTypeTranslation = new()
	{
		{ "text/csv", MimeTypeEnum.TextCsv },
		{ "text/html", MimeTypeEnum.TextHtml },
		{ "text/plain", MimeTypeEnum.TextPlain},
		{ "text/javascript", MimeTypeEnum.TextJavascript },
		{ "application/json", MimeTypeEnum.ApplicationJson },
		{ "image/jpeg", MimeTypeEnum.ImageJpeg },
		{ "image/png", MimeTypeEnum.ImagePng },
		{ "application/octet-stream", MimeTypeEnum.ApplicationOctetStream }
	};

	public readonly MimeTypeEnum Type;
	public readonly string Name;

	public MimeType(string mimeType)
	{
		Type = _mimeTypeTranslation.GetValueOrDefault(mimeType, MimeTypeEnum.ApplicationOctetStream);
		Name = mimeType;
	}
}