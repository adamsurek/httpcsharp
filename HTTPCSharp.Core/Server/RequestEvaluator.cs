using System.Net.Mime;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HTTPCSharp.Core.Requests;
using HTTPCSharp.Core.Responses;

namespace HTTPCSharp.Core.Server;

public static class RequestEvaluator
{
	private static readonly string RootDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;
	private static readonly string ResourcesPath = Path.Combine(RootDirectory, "Resources", "site");
	
	public static async Task<Response> EvaluateRequestAsync(Request request)
	{
		// TODO: Validate request 
			// TODO: Validate URI
			// TODO: Ensure that all required headers were provided
				// TODO: Validate request body based on headers?
		// TODO: Route request to handler relevant to URI
			// TODO: Create URI handlers - 2 or 3 valid base URIs with support for random sub-URIs?
		// TODO: Process the request with the corresponding handler
		// TODO: Generate response
		
		Response response;
		switch (request.RequestLine.Method)
		{
			case RequestMethodEnum.Options:
				response = await HandleOptionsRequestAsync(request.RequestLine.RequestUri);
				break;
			
			case RequestMethodEnum.Get:
				response = await HandleGetRequestAsync(request);
				break;
			
			default:
				response = new Response(new StatusLine(StatusCodesEnum.NotImplemented, "Not Implemented"),
					AddGenericHeaders(),
					$"Unknown HTTP Request Method - '{request.RequestLine.Method}'");
				break;
		}

		return response;
	}


	private static async Task<Response> HandleOptionsRequestAsync(RequestUri uri)
	{
		List<ResponseHeader> headers = AddGenericHeaders();
		string? resourcePath = GetResourcePath(uri.ToFilePath());

		if (resourcePath != null)
		{
			List<RequestMethodEnum> allowedMethods = await CheckAllowedRequestMethodsAsync(uri.Path);
			
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.Allow, string.Join(", ", allowedMethods).ToUpper()));
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, $"OPTIONS request made to '{uri}'\n");
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, $"OPTIONS request made to '{uri}' - RESOURCE NOT FOUND");
	}

	private static List<ResponseHeader> AddGenericHeaders()
	{
		return new List<ResponseHeader>()
		{
			new(HeaderFieldTypeEnum.Date, DateTime.Now.ToUniversalTime().ToString("R")),
			new(HeaderFieldTypeEnum.Server, "nunya"),
			new(HeaderFieldTypeEnum.ContentType, "text/plain"),
		};
	}

	private static string? GetResourcePath(string path)
	{
		string fullPath = Path.Combine(ResourcesPath, path);

		if (Path.Exists(fullPath))
		{
			return fullPath;
		}

		return null;
	}

	private static InternalResource GetResourceInformation(string path)
	{
		List<RequestMethodEnum> allowedMethods = new();
		
		XmlDocument sitePermissions = new();
		sitePermissions.Load(Path.Combine(ResourcesPath, "SitePermissions.xml"));

		XmlNode? resourceNode = sitePermissions.DocumentElement!.SelectSingleNode($"//HttpResource[@name='{path}']");

		if (resourceNode is null)
		{
			throw new Exception($"SITE PERMISSIONS XML IS MISSING RESOURCE NODE '{path}'");
		}

		if (resourceNode.Attributes?["type"]?.InnerText is null)
		{
			throw new Exception($"RESOURCE NODE IS MISSING TYPE ATTRIBUTE: '{resourceNode.Name}'");
		}

		string resourceTypeAttribute = resourceNode.Attributes!["type"]!.InnerText;

		bool isValidResourceType = ResourceTypeEnum.TryParse(resourceTypeAttribute, true, out ResourceTypeEnum resourceType);

		if (!isValidResourceType)
		{
			throw new Exception($"INVALID RESOURCE TYPE: '{resourceTypeAttribute}'");
		}

		XmlNode? filePropertiesNode = resourceNode.SelectSingleNode("FileProperties");

		if (filePropertiesNode is null)
		{
			throw new Exception($"FILE RESOURCE IS MISSING FILE PROPERTIES NODE: '{resourceNode.Name}'");
		}
			
		XmlNode? fileMimeTypeNode = filePropertiesNode.SelectSingleNode("MIMEType");
		if (fileMimeTypeNode is null)
		{
			throw new Exception($"FILE RESOURCE IS MISSING MIME TYPE NODE: '{filePropertiesNode.Name}'");
		}

		// string mimeTypeText = fileMimeTypeNode.InnerText;
		MimeType resourceMimeType = new(fileMimeTypeNode.InnerText);

		// bool isValidMimeType = MimeTypeEnum.TryParse(mimeTypeText, true, out resourceMimeType);
		// if (!isValidMimeType)
		// {
		// 	throw new Exception($"INVALID RESOURCE TYPE: '{resourceTypeAttribute}'");
		// }

		XmlNodeList? methodNodes = resourceNode.SelectSingleNode("AllowedRequestMethods")!.SelectNodes("Method");

		if (methodNodes is null)
		{
			allowedMethods.Add(RequestMethodEnum.Options);
		}
		else
		{
			foreach (XmlNode methodNode in methodNodes)
			{
				if (methodNode.InnerText is null)
				{
					throw new Exception($"METHOD NODE IS MISSING VALUE: '{methodNode.Value}'");
				}

				string methodNodeValue = methodNode.InnerText;

				bool isValidRequestMethod = RequestMethodEnum.TryParse(methodNodeValue, ignoreCase: true, out RequestMethodEnum method);
			
				if (!isValidRequestMethod)
				{
					throw new Exception($"UNHANDLED REQUEST METHOD: '{method}'");
				}
			
				allowedMethods.Add(method);
			}
		}

		return new InternalResource(Path.Combine(ResourcesPath, path), resourceType, resourceMimeType, allowedMethods);
	}
	
	private static async Task<InternalResource> GetResourceInformationAsync(string path)
	{
		List<RequestMethodEnum> allowedMethods = new();

		XmlReaderSettings readerSettings = new();
		readerSettings.Async = true;
		
		XmlReader reader = XmlReader.Create(Path.Combine(ResourcesPath, "SitePermissions.xml"), readerSettings);
		XDocument xml = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);
		
		XElement? resourceNode = xml.Root!.XPathSelectElement($"//HttpResource[@name='{path}']");
		// XElement? resourceNode = xml.Root!.Element($"HttpResource[@name='{path}']");
		
		if (resourceNode is null)
		{
			throw new Exception($"SITE PERMISSIONS XML IS MISSING RESOURCE NODE '{path}'");
		}

		if (resourceNode.Attribute("type") is null)
		{
			throw new Exception($"RESOURCE NODE IS MISSING TYPE ATTRIBUTE: '{resourceNode.Name}'");
		}
		
		string resourceTypeAttribute = (string)resourceNode.Attribute("type")!;
		
		bool isValidResourceType = ResourceTypeEnum.TryParse(resourceTypeAttribute, true, out ResourceTypeEnum resourceType);

		if (!isValidResourceType)
		{
			throw new Exception($"INVALID RESOURCE TYPE: '{resourceTypeAttribute}'");
		}
		
		XElement? filePropertiesNode = resourceNode.Element("FileProperties");
		
		if (filePropertiesNode is null)
		{
			throw new Exception($"FILE RESOURCE IS MISSING FILE PROPERTIES NODE: '{resourceNode.Name}'");
		}

		XElement? fileMimeTypeNode = filePropertiesNode.Element("MIMEType");
		
		if (fileMimeTypeNode is null)
		{
			throw new Exception($"FILE RESOURCE IS MISSING MIME TYPE NODE: '{filePropertiesNode.Name}'");
		}
		
		MimeType resourceMimeType = new(fileMimeTypeNode.Value);
		
		// bool isValidMimeType = MimeTypeEnum.TryParse(mimeTypeText, true, out resourceMimeType);
		// if (!isValidMimeType)
		// {
		// 	throw new Exception($"INVALID RESOURCE TYPE: '{resourceTypeAttribute}'");
		// }
		

		IEnumerable<XElement> methodNodes = resourceNode.Elements("AllowedRequestMethods").First().Elements("Method");

		if (!methodNodes.ToList().Any())
		{
			allowedMethods.Add(RequestMethodEnum.Options);
		}
		else
		{
			foreach (XElement methodNode in methodNodes)
			{
				if (methodNode.Value is null)
				{
					throw new Exception($"METHOD NODE IS MISSING VALUE: '{methodNode.Value}'");
				}

				string methodNodeValue = methodNode.Value;
				RequestMethodEnum method;
			
				bool isValidRequestMethod = RequestMethodEnum.TryParse(methodNodeValue, ignoreCase: true, out method);
			
				if (!isValidRequestMethod)
				{
					throw new Exception($"UNHANDLED REQUEST METHOD: '{method}'");
				}
			
				allowedMethods.Add(method);
			}
		}

		return new InternalResource(Path.Combine(ResourcesPath, path), resourceType, resourceMimeType, allowedMethods);
	}

	private static List<RequestMethodEnum> CheckAllowedRequestMethods(string path)
	{
		List<RequestMethodEnum> allowedMethods = new();
		
		XmlDocument sitePermissions = new();
		sitePermissions.Load(Path.Combine(ResourcesPath, "SitePermissions.xml"));

		XmlNode? resourceNode = sitePermissions.DocumentElement!.SelectSingleNode($"//HttpResource[@name='{path}']");

		if (resourceNode is null)
		{
			throw new Exception($"SITE PERMISSIONS XML IS MISSING RESOURCE NODE '{path}'");
		}

		XmlNodeList? methodNodes = resourceNode.SelectSingleNode("AllowedRequestMethods")!.SelectNodes("Method");

		if (methodNodes is null)
		{
			allowedMethods.Add(RequestMethodEnum.Options);
			return allowedMethods;
		}
		
		Console.WriteLine(string.Join("\n", methodNodes.Count));

		foreach (XmlNode methodNode in methodNodes)
		{
			if (methodNode.InnerText is null)
			{
				throw new Exception($"METHOD NODE IS MISSING VALUE: '{methodNode.Value}'");
			}

			string methodNodeValue = methodNode.InnerText;
			RequestMethodEnum method;
			
			bool isValidRequestMethod = RequestMethodEnum.TryParse(methodNodeValue, ignoreCase: true, out method);
			
			if (!isValidRequestMethod)
			{
				throw new Exception($"UNHANDLED REQUEST METHOD: '{method}'");
			}
			
			Console.WriteLine($"PARSED METHOD: {method}");
			allowedMethods.Add(method);
		}

		return allowedMethods;
	}

	public static async Task<List<RequestMethodEnum>> CheckAllowedRequestMethodsAsync(string path)
	{
		List<RequestMethodEnum> allowedMethods = new();

		XmlReader reader = XmlReader.Create(Path.Combine(ResourcesPath, "SitePermissions.xml"));
		XDocument xml = await XDocument.LoadAsync(reader, LoadOptions.None, CancellationToken.None);

		XElement? resourceNode = xml.Root!.Element($"//HttpResource[@name='{path}']");
		
		if (resourceNode is null)
		{
			throw new Exception($"SITE PERMISSIONS XML IS MISSING RESOURCE NODE '{path}'");
		}

		IEnumerable<XElement> methodNodes = resourceNode.Elements("AllowedRequestMethods").Elements("Method");

		if (!methodNodes.ToList().Any())
		{
			allowedMethods.Add(RequestMethodEnum.Options);
			return allowedMethods;
		}
		
		Console.WriteLine(string.Join("\n", methodNodes.Count()));

		foreach (XElement methodNode in methodNodes)
		{
			if (methodNode.Value is null)
			{
				throw new Exception($"METHOD NODE IS MISSING VALUE: '{methodNode.Value}'");
			}

			string methodNodeValue = methodNode.Value;
			RequestMethodEnum method;
			
			bool isValidRequestMethod = RequestMethodEnum.TryParse(methodNodeValue, ignoreCase: true, out method);
			
			if (!isValidRequestMethod)
			{
				throw new Exception($"UNHANDLED REQUEST METHOD: '{method}'");
			}
			
			Console.WriteLine($"PARSED METHOD: {method}");
			allowedMethods.Add(method);
		}

		return allowedMethods;
		
	}

	private static async Task<Response> HandleGetRequestAsync(Request request)
	{
		List<ResponseHeader> headers = new()
		{
			new(HeaderFieldTypeEnum.Date, DateTime.Now.ToUniversalTime().ToString("R")),
			new(HeaderFieldTypeEnum.Server, "nunya")
		};
		string? resourcePath = GetResourcePath(request.RequestLine.RequestUri.ToFilePath());

		if (resourcePath != null)
		{
			InternalResource resource = await GetResourceInformationAsync(request.RequestLine.RequestUri.Path);

			if (!resource.AllowedRequestMethods.Contains(RequestMethodEnum.Get))
			{
				headers.Add(new ResponseHeader(HeaderFieldTypeEnum.Allow, string.Join(", ", resource.AllowedRequestMethods).ToUpper()));
				return new Response(new StatusLine(StatusCodesEnum.MethodNotAllowed, "Method Not Allowed"), headers,
					$"GET request made to '{request.RequestLine.RequestUri.Path}' - HTTP METHOD NOT ALLOWED");
			}

			string body;
			if (resource.MimeType.Name.StartsWith("image"))
			{
				body = Encoding.ASCII.GetString(await File.ReadAllBytesAsync(resourcePath));
			}
			else
			{
				 body = await File.ReadAllTextAsync(resourcePath);
			}
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentLength, body.Length.ToString()));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.AcceptRanges, "bytes"));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentType, resource.MimeType.Name));
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, body);
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, $"OPTIONS request made to '{request.RequestLine.RequestUri.Path}' - RESOURCE NOT FOUND");
	}

}