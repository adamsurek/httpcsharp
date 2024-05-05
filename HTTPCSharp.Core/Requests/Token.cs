namespace HTTPCSharp.Core.Requests;

public class Token
{
	public TokenEnum TokenType;
	public string TokenLiteral;

	public Token(TokenEnum tokenType, string tokenLiteral)
	{
		TokenType = tokenType;
		TokenLiteral = tokenLiteral;
	}
}