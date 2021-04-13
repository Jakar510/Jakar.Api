using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.Networking
{
	public class HeaderException : Exception
	{
		public HttpRequestHeader Key { get; }
		public Type Actual { get; }
		public IList<Type> Expected { get; }

		public HeaderException( HttpRequestHeader key, Type actual, params Type[] expected ) : base(GetMessage(key, actual, expected))
		{
			Key = key;
			Actual = actual;
			Expected = expected;

			Data[nameof(Key)] = Key.ToString();
			Data[nameof(Actual)] = Actual.FullName;
			Data[nameof(Expected)] = GetTypes(expected);
		}

		protected static string GetTypes( params Type[] expected ) => expected.Aggregate("", ( current, item ) => current + @$"""{item.FullName}"", ");

		protected static string GetMessage( HttpRequestHeader key, Type actual, params Type[] expected )
		{
			return @$"For the HttpRequestHeader ""{key}"", the value passed needs to be of following types: [ {GetTypes(expected)} ] but got ""{actual.FullName}"" ";
		}
	}
}
