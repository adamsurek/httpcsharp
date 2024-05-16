using System.Globalization;
using HTTPCSharp.Core.Requests;
using HTTPCSharp.Core.Responses;

namespace HTTPCSharp.Core.Server;

public static class RequestEvaluator
{
	public static Response EvaluateRequest(Request request)
	{
		Response response;
		switch (request.RequestLine.Method)
		{
			case RequestMethodEnum.Options:
				response = HandleOptionsRequest(request.RequestLine.RequestUri);
				break;
			
			// case RequestMethodEnum.Get:
			// 	HandleGetRequest(request);
			// 	break;
			
			default:
				ResponseHeader header = new("Date", DateTime.Now.ToUniversalTime().ToString(CultureInfo.CurrentCulture));
				response = new Response(new StatusLine(StatusCodesEnum.InternalServerError, "Internal Server Error"),
					new List<ResponseHeader>() { header },
					$"Unknown HTTP Request Method - '{request.RequestLine.Method}'");
				break;
		}

		return response;
	}

	private static Response HandleOptionsRequest(RequestUri uri)
	{
		ResponseHeader header = new("Date", DateTime.Now.ToUniversalTime().ToString("R"));
		return new Response(new StatusLine(StatusCodesEnum.Ok                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  , "OK"), new List<ResponseHeader>() {header}, $"OPTIONS request made to '{uri}'");
	}
	
	private static void HandleGetRequest(Request getRequest)
	{
		
	}
}