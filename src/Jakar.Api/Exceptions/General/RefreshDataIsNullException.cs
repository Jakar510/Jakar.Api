using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class RefreshDataIsNullException : Exception
	{
		public RefreshDataIsNullException() { }
		public RefreshDataIsNullException( string message ) : base(message) { }
		public RefreshDataIsNullException( string message, Exception inner ) : base(message, inner) { }
	}
}
