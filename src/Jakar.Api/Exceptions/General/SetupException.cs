using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class SetupException : Exception
	{
		public SetupException() { }
		public SetupException( string message ) : base(message) { }
		public SetupException( string message, Exception inner ) : base(message, inner) { }
	}
}