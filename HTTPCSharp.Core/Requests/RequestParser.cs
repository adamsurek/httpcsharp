using System.Text;
using System.Xml;

namespace HTTPCSharp.Core.Requests;

public class RequestParser
{
	private readonly byte[] _requestBuffer;
	private int _currentByteIndex;
	private byte _currentByte;
	private byte _nextByte;

	private const byte Null = 0;
	private const byte HorizontalTab = 9;
	private const byte LineFeed = 10;
	private const byte CarriageReturn = 13;
	private const byte Space = 32;
	private const byte Pound = 35;
	private const byte Asterisk = 42;
	private const byte Dot = 46;
	private const byte ForwardSlash = 47;
	private const byte Colon = 58;
	private const byte Semicolon = 59;
	private const byte QuestionMark = 63;
	
	private readonly Dictionary<string, RequestMethodEnum> _requestMethodMap = new()
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

	public RequestParser(byte[] requestBuffer)
	{
		_requestBuffer = requestBuffer;
		_currentByteIndex = 0;
		_currentByte = requestBuffer[0];
		_nextByte = requestBuffer[1];
	}

	public Request Parse()
	{
		RequestLine requestLine = ParseRequestLine();
		
		NextByte(); // CR
		NextByte(); // LF

		List<RequestHeader> requestHeaders = new();

		while (true)
		{
			if (_nextByte == CarriageReturn)
			{
				NextByte();
				NextByte();

				// When the header section ends, there are two sets of CRLF. If the current
				// byte is a LF and the next byte is a LF, we have reached the body.
				if (_currentByte == LineFeed && _nextByte == CarriageReturn)
				{
					NextByte();
					NextByte();
					break;
				}
			}
			
			RequestHeader requestHeader = ParseRequestHeader();
			requestHeaders.Add(requestHeader);
		}
			
		string? requestBody = null;
		if (_currentByteIndex <= _requestBuffer.Length && _currentByte != Null || _nextByte != Null)
		{
			requestBody = ParseRequestBody();
		}
		
		return new Request(requestLine, requestHeaders, requestBody);
	}
	
	private RequestLine ParseRequestLine()
	{
		// Parse Method
		RequestMethodEnum method = ParseHttpMethod();
		
		// Parse URI
		RequestUri uri = ParseRequestUri();
		
		//Parse Version
		HttpVersion version = ParseHttpVersion();
		
		return new RequestLine(method, uri, version);
	}

	private RequestMethodEnum ParseHttpMethod()
	{
		while (_currentByte == Space || _currentByte == HorizontalTab)
		{
			NextByte();
		}

		int startIndex = _currentByteIndex;
		while (_nextByte != Space && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}

		string translatedMethod = Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
		bool isValidMethod = _requestMethodMap.TryGetValue(translatedMethod.ToUpper(), out RequestMethodEnum method);

		if (!isValidMethod)
		{
			method = RequestMethodEnum.Unknown;
		}
		
		NextByte(); // Skip space
		return method;
	}

	private void NextByte()
	{
		_currentByteIndex++;
		_currentByte = _nextByte;
		_nextByte = _requestBuffer[_currentByteIndex];
	}

	private RequestUri ParseRequestUri()
	{
		if (_currentByte == Asterisk)
		{
			NextByte();
			return new RequestUri(null, null, -1, "*", null, null);
		}

		string path = ParseUriPath();

		string? query = null;
		if (_nextByte == QuestionMark)
		{
			NextByte();
			query = ParseUriQuery();
		}
		
		NextByte(); // Skip space
		return new RequestUri(null, null, -1, path, query, null);
	}

	private string ParseUriPath()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != Space && _nextByte != QuestionMark && _nextByte != Pound)
		{
			NextByte();
		}
		
		return Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
	}

	private string ParseUriQuery()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != Space && _nextByte != Pound)
		{
			NextByte();
		}
		
		return Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
	}

	private HttpVersion ParseHttpVersion()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != ForwardSlash && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}

		string httpType = Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
		if (httpType.ToUpper() != "HTTP")
		{
			throw new Exception($"ONLY HTTP SUPPORTED, RECEIVED {httpType}");
		}
		
		NextByte();
		
		startIndex = _currentByteIndex;
		while (_nextByte != Dot && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}

		int majorVersion = int.Parse(_requestBuffer.AsSpan()[startIndex.._currentByteIndex]);
		NextByte();
		
		startIndex = _currentByteIndex;
		while (_nextByte != CarriageReturn && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}
		
		int minorVersion = int.Parse(_requestBuffer.AsSpan()[startIndex.._currentByteIndex]);

		return new HttpVersion(majorVersion, minorVersion);
	}

	private RequestHeader ParseRequestHeader()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != Colon)
		{
			NextByte();
		}

		string headerType = Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);

		while (_nextByte == Colon || _nextByte == Space || _nextByte == HorizontalTab)
		{
			NextByte();
		}

		startIndex = _currentByteIndex;
		while (_nextByte != CarriageReturn)
		{
			NextByte();
		}
		
		string headerValue = Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);

		return new RequestHeader(headerType, headerValue);
	}

	private string ParseRequestBody()
	{
		int startIndex = _currentByteIndex;
		while (_currentByte != Null && _nextByte != Null && _currentByteIndex + 1 < _requestBuffer.Length)
		{
			NextByte();
		}
		
		return Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
	}
}