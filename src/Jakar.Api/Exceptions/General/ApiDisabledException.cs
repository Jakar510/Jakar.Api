using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class ApiDisabledException : Exception
	{
		public ApiDisabledException() { }
		public ApiDisabledException( string message ) : base(message) { }
		public ApiDisabledException( string message, Exception inner ) : base(message, inner) { }
	}
}
