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
					Encoding.UTF8.GetBytes($"Unknown HTTP Request Method - '{request.RequestLine.Method}'"));
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
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, Encoding.UTF8.GetBytes($"OPTIONS request made to '{uri}'\n"));
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, Encoding.UTF8.GetBytes($"OPTIONS request made to '{uri}' - RESOURCE NOT FOUND"));
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
					Encoding.UTF8.GetBytes($"GET request made to '{uri}' - HTTP METHOD NOT ALLOWED"));
			}

			byte[] body;
			string filePath = Path.Combine(_resourcesPath, uri.ToFilePath());
			
			try
			{
				body = await ReadFileToBytes(filePath);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentLength, body.Length.ToString()));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.AcceptRanges, "bytes"));
			headers.Add(new ResponseHeader(HeaderFieldTypeEnum.ContentType, resource.MimeType.Name));
			return new Response(new StatusLine(StatusCodesEnum.Ok, "OK"), headers, body);
		}
		
		return new Response(new StatusLine(StatusCodesEnum.NotFound, "Not Found"), headers, Encoding.UTF8.GetBytes($"OPTIONS request made to '{uri}' - RESOURCE NOT FOUND"));
	}

	private async Task<byte[]> ReadFileToBytes(string filePath)
	{
		FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
		byte[] contents = new byte[stream.Length];
		int bytesRead;
		int offset = 0;
		int chunkSize = int.Min((int)stream.Length, 4096);

		while ((bytesRead = await stream.ReadAsync(contents, offset, chunkSize)) > 0)
		{
			offset += bytesRead;
			chunkSize = int.Min((int)stream.Length - bytesRead, 4096);
		}
		
		stream.Close();
		return contents;
	}

}