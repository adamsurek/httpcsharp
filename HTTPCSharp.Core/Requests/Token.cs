namespace HTTPCSharp.Core.Requests;

public class Token
{
	public TokenEnum TokenType;
	public byte[] TokenLiteral;

	public Token(TokenEnum tokenType, byte[] tokenLiteral)
	{
		TokenType = tokenType;
		TokenLiteral = tokenLiteral;
	}
}