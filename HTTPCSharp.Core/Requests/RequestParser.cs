using System.Text;

namespace HTTPCSharp.Core.Requests;

public class RequestParser
{
	private byte[] _requestBuffer;
	private int _currentByteIndex;
	private byte _currentByte;
	private byte _nextByte;
	
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

	public RequestParser(byte[] requestBuffer)
	{
		_requestBuffer = requestBuffer;
		_currentByteIndex = 0;
		_currentByte = requestBuffer[0];
		_nextByte = requestBuffer[1];
	}

	public Request Parse()
	{
		Console.WriteLine("### PARSER ATTEMPT TWO ###");
		RequestLine requestLine = ParseRequestLine();

		return new Request(requestLine);
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
		while (_currentByte == 32 || _currentByte == 9)
		{
			NextByte();
		}

		int startIndex = _currentByteIndex;
		while (_nextByte != 32 && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}

		string translatedMethod = Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
		bool isValidMethod = _requestMethodMap.TryGetValue(translatedMethod.ToUpper(), out RequestMethodEnum method);

		if (!isValidMethod)
		{
			throw new Exception($"INVALID HTTP METHOD - '{translatedMethod}'");
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
		if (_currentByte == 42)
		{
			NextByte();
			return new RequestUri(null, null, -1, "*", null, null);
		}

		// if (_currentByte == 47)
		// {
		string path = ParseUriPath();
		// }

		string? query = null;
		if (_nextByte == 63)
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
		while (_nextByte != 32 && _nextByte != 63 && _nextByte != 35)
		{
			NextByte();
		}
		
		return Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
	}

	private string ParseUriQuery()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != 32 && _nextByte != 35)
		{
			NextByte();
		}
		
		return Encoding.ASCII.GetString(_requestBuffer[startIndex.._currentByteIndex]);
	}

	private HttpVersion ParseHttpVersion()
	{
		int startIndex = _currentByteIndex;
		while (_nextByte != 47 && _currentByteIndex < _requestBuffer.Length)
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
		while (_nextByte != 46 && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}

		int majorVersion = int.Parse(_requestBuffer.AsSpan()[startIndex.._currentByteIndex]);
		NextByte();
		
		startIndex = _currentByteIndex;
		while (_nextByte != 13 && _currentByteIndex < _requestBuffer.Length)
		{
			NextByte();
		}
		
		int minorVersion = int.Parse(_requestBuffer.AsSpan()[startIndex.._currentByteIndex]);

		return new HttpVersion(majorVersion, minorVersion);
	}
}