using System.Text;

namespace HTTPCSharp.Core.Requests;

public enum TokenEnum
{
	Alpha = -2,
	Digit = -1,
	Null = 0,
	HorizontalTab = 9,
	LineFeed = 10,
	CarriageReturn = 13,
	Space = 32,
	Dot = 46,
	Slash = 47,
	Colon = 58,
	Semicolon = 59,
}