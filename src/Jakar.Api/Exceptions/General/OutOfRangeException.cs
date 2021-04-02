using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class OutOfRangeException : ArgumentOutOfRangeException
	{
		public OutOfRangeException() { }
		public OutOfRangeException( string message ) : base(message) { }
		public OutOfRangeException( string paramName, string message ) : base(paramName, message) { }
		public OutOfRangeException( string message, Exception inner ) : base(message, inner) { }
		public OutOfRangeException( string paramName, object actualValue, string message ) : base(paramName, actualValue, message) { }
	}
}