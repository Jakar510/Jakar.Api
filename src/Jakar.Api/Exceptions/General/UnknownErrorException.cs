// unset

using System;

namespace Jakar.Api.Exceptions.General
{
	public class UnknownErrorException : Exception
	{
		public UnknownErrorException() { }
		public UnknownErrorException( string message ) : base(message) { }
		public UnknownErrorException( string message, Exception inner ) : base(message, inner) { }
	}
}