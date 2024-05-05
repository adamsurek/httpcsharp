namespace HTTPCSharp.Core.Requests;

public class HttpVersion
{
	public const string Http = "HTTP";
	public readonly int MajorVersion;
	public readonly int MinorVersion;

	public HttpVersion(int majorVersion, int minorVersion)
	{
		MajorVersion = majorVersion;
		MinorVersion = minorVersion;
	}

	public override string ToString()
	{
		return $"{Http}/{MajorVersion}.{MinorVersion}";
	}
}