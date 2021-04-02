using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class NotLoggedInException : Exception
	{
		public NotLoggedInException() { }
		public NotLoggedInException( string message ) : base(message) { }
		public NotLoggedInException( string message, Exception inner ) : base(message, inner) { }
	}
}