using System.Text;

namespace HTTPCSharp.Core.Requests;

public enum TokenEnum
{
	String = -1,
	Null = '\0',
	HorizontalTab = '\t',
	LineFeed = '\n',
	CarriageReturn = '\r',
	Space = '\u0020',
	Colon = ':',
	Semicolon = ';',
	Slash = '/',
	Dot = '.',
}