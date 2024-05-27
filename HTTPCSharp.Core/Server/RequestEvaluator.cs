using System.Text;
using HTTPCSharp.Core.Requests;
using HTTPCSharp.Core.Responses;

namespace HTTPCSharp.Core.Server;

public class RequestEvaluator
{
	private readonly string _rootDirectory;
	private readonly string _resourcesPath;
	
	// Load Resources/Site Permissions
	private readonly List<InternalResource> _internalResources;

	public RequestEvaluator(ServerConfig config)
	{
		_rootDirectory = config.RootDirectory;
		_resourcesPath = config.ResourcesPath;
		_internalResources = config.InternalResources;
	}

	public async Task<Response> EvaluateRequestAsync(Request request)
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
			
			case RequestMethodEnum.Get:
				response = await HandleGetRequest(request.RequestLine.RequestUri);
				break;
			
			default:
				response = new Response(new StatusLine(StatusCodesEnum.NotImplemented, "Not Implemented"),
					AddGenericHeaders(),
					$"Unknown HTTP Request Method - '{request.RequestLine.Method}'");
				break;
		}

		return response;
	}

	private Response HandleOptionsRequest(RequestUri uri)
	{
		List<ResponseHeader> headers = AddGenericHeaders();

		InternalResource? resource =
			_internalResources.Find(resource => resource.ResourcePath == uri.Path);
		
		if (resource is not null)
		{
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.Allow, string.Join(", ", resource.AllowedRequestMethods).ToUpper()));
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, $"OPTIONS request made to '{uri}'\n");
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, $"OPTIONS request made to '{uri}' - RESOURCE NOT FOUND");
	}

	private List<ResponseHeader> AddGenericHeaders()
	{
		return new List<ResponseHeader>()
		{
			new(HeaderFieldTypeEnum.Date, DateTime.Now.ToUniversalTime().ToString("R")),
			new(HeaderFieldTypeEnum.Server, "nunya"),
			new(HeaderFieldTypeEnum.ContentType, "text/plain"),
		};
	}

	private async Task<Response> HandleGetRequest(RequestUri uri)
	{
		List<ResponseHeader> headers = new()
		{
			new(HeaderFieldTypeEnum.Date, DateTime.Now.ToUniversalTime().ToString("R")),
			new(HeaderFieldTypeEnum.Server, "nunya")
		};
		
		InternalResource? resource =
			_internalResources.Find(resource => resource.ResourcePath == uri.Path);
		
		if (resource != null)
		{
			if (!resource.AllowedRequestMethods.Contains(RequestMethodEnum.Get))
			{
				headers.Add(new ResponseHeader(HeaderFieldTypeEnum.Allow, string.Join(", ", resource.AllowedRequestMethods).ToUpper()));
				return new Response(new StatusLine(StatusCodesEnum.MethodNotAllowed, "Method Not Allowed"), headers,
					$"GET request made to '{uri}' - HTTP METHOD NOT ALLOWED");
			}

			string body;
			string filePath = Path.Combine(_resourcesPath, uri.ToFilePath());
			if (resource.MimeType.Name.StartsWith("image"))
			{
				body = Encoding.ASCII.GetString(await File.ReadAllBytesAsync(filePath));
			}
			else
			{
				 body = await File.ReadAllTextAsync(filePath);
			}
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentLength, body.Length.ToString()));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.AcceptRanges, "bytes"));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentType, resource.MimeType.Name));
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, body);
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, $"OPTIONS request made to '{uri}' - RESOURCE NOT FOUND");
	}

}