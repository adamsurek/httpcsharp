using System.Xml;
using HTTPCSharp.Core.Requests;
using HTTPCSharp.Core.Responses;

namespace HTTPCSharp.Core.Server;

public static class RequestEvaluator
{
	private static readonly string RootDirectory = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;
	private static readonly string ResourcesPath = Path.Combine(RootDirectory, "Resources", "site");
	
	public static Response EvaluateRequest(Request request)
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
				response = HandleOptionsRequest(request.RequestLine.RequestUri);
				break;
			
			default:
				response = new Response(new StatusLine(StatusCodesEnum.NotImplemented, "Not Implemented"),
					AddGenericHeaders(),
					$"Unknown HTTP Request Method - '{request.RequestLine.Method}'");
				break;
		}

		return response;
	}


	private static Response HandleOptionsRequest(RequestUri uri)
	{
		List<ResponseHeader> headers = AddGenericHeaders();
		string? resourcePath = GetResourcePath(uri.ToFilePath());

		if (resourcePath != null)
		{
			List<RequestMethodEnum> allowedMethods = CheckAllowedRequestMethods(uri.Path);
			
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
}