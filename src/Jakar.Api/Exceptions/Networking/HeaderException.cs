using System;
using System.Net;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.Networking
{
	public class HeaderException : Exception
	{
		public HttpRequestHeader Key { get; }
		public Type Actual { get; }
		public Type Expected { get; }

		public HeaderException( HttpRequestHeader key, Type actual, Type expected ) : base(GetMessage(key, actual, expected))
		{
			Key = key;
			Actual = actual;
			Expected = expected;

			Data[nameof(Key)] = Key.ToString();
			Data[nameof(Actual)] = Actual.FullName;
			Data[nameof(Expected)] = Expected.FullName;
		}

		protected static string GetMessage( HttpRequestHeader key, Type actual, Type expected )
		{
			return @$"For the HttpRequestHeader ""{key}"", the value passed needs to be of type ""{expected.FullName}"" but got ""{actual.FullName}"" ";
		}
	}
}
