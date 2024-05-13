using System.Text;

namespace HTTPCSharp.Core.Requests;

public class Lexer
{
	private readonly byte[] _input;
	private int _position;
	private int _readPosition;
	private char _currentChar;

	public Lexer(byte[] input)
	{
		_input = input;
		ReadByte();
	}

	public Token NextToken()
	{
		Token token = new(TokenEnum.Null, '\0'.ToString());
		
		switch (_currentChar)
		{
			case '\t':
				token = new (TokenEnum.HorizontalTab, _currentChar.ToString());
				break;
			
			case '\n':
				token = new(TokenEnum.LineFeed, _currentChar.ToString());
				break;
			
			case '\r':
				token = new(TokenEnum.CarriageReturn, _currentChar.ToString());
				break;
			
			case ' ':
				token = new(TokenEnum.Space, _currentChar.ToString());
				break;
			
			case ':':
				token = new(TokenEnum.Colon, _currentChar.ToString());
				break;
			
			case ';':
				token = new(TokenEnum.Semicolon, _currentChar.ToString());
				break;
			
			case '/':
				token = new(TokenEnum.Slash, _currentChar.ToString());
				break;
			
			case '.':
				token = new(TokenEnum.Dot, _currentChar.ToString());
				break;
			
			case '\0':
				token = new Token(TokenEnum.Null, _currentChar.ToString());
				break;
			
			default:
				if (Char.IsDigit(_currentChar))
				{
					token = new (TokenEnum.String, ReadInteger().ToString());
				}
				else
				{
					token = new (TokenEnum.String, ReadString());
				}
				break;
		}
		
		ReadByte();
		
		return token;
	}

	private void ReadByte()
	{
		if (_readPosition >= _input.Length)
		{
			_currentChar = (char)TokenEnum.Null;
		}
		else
		{
			_currentChar = (char)_input[_readPosition];
		}

		_position = _readPosition;
		_readPosition += 1;
	}

	private string ReadString()
	{
		int startPosition = _position;
		// TODO: Improve this - seems janky
		while (_currentChar != ':' && _currentChar != ' ' && _currentChar != '/' && _currentChar != '\r' && _currentChar != '\0')
		{
			ReadByte();
		}
		
		return Encoding.UTF8.GetString(_input[startPosition..(_position)]);
	}


	private int ReadInteger()
	{
		int startPosition = _position;
		// TODO: Improve this - also seems janky
		while (_currentChar != '.' && _currentChar != '\0' && Char.IsAsciiDigit(_currentChar))
		{
			ReadByte();
		}

		return int.Parse(Encoding.UTF8.GetString(_input[startPosition.._position]));
	}
}