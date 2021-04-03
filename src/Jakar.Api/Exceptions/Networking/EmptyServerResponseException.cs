using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.Networking
{
	public class EmptyServerResponseException : Exception
	{
		public EmptyServerResponseException() { }

		public EmptyServerResponseException( string message ) : base(message) { }

		public EmptyServerResponseException( string message, Exception inner ) : base(message, inner) { }
	}
}
