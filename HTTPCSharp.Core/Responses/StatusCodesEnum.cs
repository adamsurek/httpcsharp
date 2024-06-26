﻿namespace HTTPCSharp.Core.Responses;

public enum StatusCodesEnum
{
	Ok = 200,
	Created = 201,
	NoContent = 204,
	BadRequest = 400,
	Unauthorized = 401,
	Forbidden = 403,
	NotFound = 404,
	MethodNotAllowed = 405,
	InternalServerError = 500,
	NotImplemented = 501,
	HttpVersionNotSupported = 505
}