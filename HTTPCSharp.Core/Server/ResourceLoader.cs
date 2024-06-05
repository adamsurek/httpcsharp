using System.Xml;
using System.Xml.Linq;
using HTTPCSharp.Core.Requests;

namespace HTTPCSharp.Core.Server;

public static class ResourceLoader
{
	public static List<InternalResource> Load(string resourceFilePath)
	{
		List<InternalResource> resources = new();
		
		XmlReader reader = XmlReader.Create(Path.Combine(resourceFilePath, "SitePermissions.xml"));
		XDocument xml = XDocument.Load(reader, LoadOptions.None);
		
		if (xml.Root is null || xml.Root.Name != "HttpResources")
		{
			throw new Exception($"INVALID SITE PERMISSIONS XML ROOT: {xml.Root}");
		}

		List<XElement> resourceNodes = xml.Root.Elements("HttpResource").ToList();
		foreach (XElement node in resourceNodes)
		{
			(string? resourceName, ResourceTypeEnum? resourceType) = GetResourceAttributes(node);
			if (resourceName is null || resourceType is null)
			{
				throw new Exception($"INVALID RESOURCE NODE: {node}");
			}

			MimeType? mimeType;
			if (resourceType is ResourceTypeEnum.File)
			{
				mimeType = GetFileProperties(node);

				if (mimeType is null)
				{
					throw new Exception($"FILE RESOURCE IS MISSING MIME TYPE NODE: '{node.Name}'");
				}
			}
			else
			{
				mimeType = new MimeType("");
			}
			
			List<RequestMethodEnum>? requestMethods = GetAllowedRequestMethods(node);
			if (requestMethods is null || requestMethods.Count == 0)
			{
				throw new Exception($"INVALID OR MISSING ALLOWED REQUEST METHODS NODE: '{node.Name}'");
			}
			
			resources.Add(new InternalResource(
				resourceName,
				(ResourceTypeEnum)resourceType,
				mimeType,
				requestMethods
				));
		}

		return resources;
	}

	private static (string?, ResourceTypeEnum?) GetResourceAttributes(XElement resourceNode)
	{
		if (resourceNode.Attribute("name") is null || resourceNode.Attribute("type") is null )
		{
			return (null, null);
		}

		// Not null at this point - need bangs to satisfy compiler
		Enum.TryParse(resourceNode.Attribute("type")!.Value, true, out ResourceTypeEnum resourceType);
		return (resourceNode.Attribute("name")!.Value, resourceType);
	}

	private static MimeType? GetFileProperties(XElement resourceNode)
	{
		XElement? filePropertiesNode = resourceNode.Element("FileProperties");
		if (filePropertiesNode is null)
		{
			return null;
		}
		
		XElement? fileMimeTypeNode = filePropertiesNode.Element("MIMEType");
		if (fileMimeTypeNode is null)
		{
			return null;
		}
		
		return new MimeType(fileMimeTypeNode.Value);
	}

	private static List<RequestMethodEnum>? GetAllowedRequestMethods(XElement resourceNode)
	{
		List<RequestMethodEnum> allowedMethods = new();
		
		XElement? allowedMethodsNode = resourceNode.Element("AllowedRequestMethods");
		if (allowedMethodsNode is null)
		{
			return null;
		}

		List<XElement> methodNodes = allowedMethodsNode.Elements("Method").ToList();
		if (methodNodes.Count == 0)
		{
			allowedMethods.Add(RequestMethodEnum.Options);
			return allowedMethods;
		}
		
		foreach (XElement methodNode in methodNodes)
		{
			if (methodNode.Value == "")
			{
				return null;
			}

			bool isValidRequestMethod = Enum.TryParse(methodNode.Value, true, out RequestMethodEnum method);
			if (!isValidRequestMethod)
			{
				return null;
			}
			
			allowedMethods.Add(method);
		}

		return allowedMethods;
	}
}