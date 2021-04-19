using System;
using System.Collections;
using System.Net;
using Jakar.Api.Exceptions.General;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.Networking
{
	public class HeaderException : ExpectedValueTypeException<HttpRequestHeader>
	{
		public HeaderException( HttpRequestHeader name, object value, Type type ) : this(name, value.GetType(), type) { }

		public HeaderException( HttpRequestHeader key, Type actual, params Type[] expected ) : base(key, actual, expected) { }
	}
}
