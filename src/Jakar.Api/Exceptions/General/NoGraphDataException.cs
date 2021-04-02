using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class NoGraphDataException : Exception
	{
		public NoGraphDataException() { }
		public NoGraphDataException( string message ) : base(message) { }
		public NoGraphDataException( string message, Exception inner ) : base(message, inner) { }
	}
}