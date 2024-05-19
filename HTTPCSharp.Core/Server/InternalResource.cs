using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Core.Server;

public enum ResourceTypeEnum
{
	Directory,
	File
}

public class InternalResource
{
	public readonly string ResourcePath;
	public readonly ResourceTypeEnum ResourceType; 
	public readonly MimeType MimeType;
	public readonly List<RequestMethodEnum> AllowedRequestMethods;

	public InternalResource(string resourcePath, ResourceTypeEnum resourceType, MimeType mimeType, List<RequestMethodEnum> allowedRequestMethods)
	{
		ResourcePath = resourcePath;
		ResourceType = resourceType;
		MimeType = mimeType;
		AllowedRequestMethods = allowedRequestMethods;
	}
}