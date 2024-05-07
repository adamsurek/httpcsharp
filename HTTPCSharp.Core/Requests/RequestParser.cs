using System.ComponentModel.DataAnnotations;

namespace HTTPCSharp.Core.Requests;

public class RequestParser
{
	private Token _currentToken;
	private Token _peekToken;
	private readonly Lexer _lexer;

	private Dictionary<string, RequestMethodEnum> _requestMethodMap = new()
	{
		{ "OPTIONS", RequestMethodEnum.Options },
		{ "GET", RequestMethodEnum.Get },
		{ "HEAD", RequestMethodEnum.Head },
		{ "POST", RequestMethodEnum.Post },
		{ "PUT", RequestMethodEnum.Put },
		{ "DELETE", RequestMethodEnum.Delete },
		{ "TRACE", RequestMethodEnum.Trace },
		{ "CONNECT", RequestMethodEnum.Connect },
	};

	public RequestParser(Lexer lexer)
	{
		_lexer = lexer;
		NextToken();
		NextToken();
	}

	private void NextToken()
	{
		_currentToken = _peekToken;
		_peekToken = _lexer.NextToken();
	}

	public Request Parse()
	{
		

		// while (_currentToken.TokenType != TokenEnum.CarriageReturn && _peekToken.TokenType != TokenEnum.LineFeed)
		// {
		RequestLine requestLine = ParseRequestLine();
		// }

		return new Request(requestLine);
	}

	private RequestLine ParseRequestLine()
	{
		RequestMethodEnum method = ParseHttpMethod();
		
		string requestUri = _currentToken.TokenLiteral;
		
		NextToken(); // SP

		HttpVersion httpVersion = ParseHttpVersion();
		
		NextToken(); // LF
		
		return new RequestLine(method, requestUri, httpVersion);
	}

	public RequestMethodEnum ParseHttpMethod()
	{
		bool isValidMethod = _requestMethodMap.TryGetValue(
			_currentToken.TokenLiteral.ToUpper(),
			out RequestMethodEnum method);
		
		if (!isValidMethod)
		{
			throw new Exception("INVALID HTTP METHOD");
		}
		
		NextToken();
		
		return method;
	}

	public HttpVersion ParseHttpVersion()
	{
		NextToken(); // Move to HTTP
		NextToken(); // Move to major version
		int majorVersion = int.Parse(_currentToken.TokenLiteral);
		
		NextToken(); // Move to dot
		
		int minorVersion  = int.Parse(_currentToken.TokenLiteral);
		
		return new HttpVersion(majorVersion, minorVersion);
	}
}