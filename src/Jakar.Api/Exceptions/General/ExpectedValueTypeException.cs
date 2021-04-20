using System;
using System.Collections.Generic;
using System.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class ExpectedValueTypeException<TKey> : Exception // Jakar.Api.Exceptions.Networking.HeaderException
	{
		public TKey Key { get; }
		public Type Actual { get; }
		public IList<Type> Expected { get; }

		public ExpectedValueTypeException( TKey name, object value, params Type[] expected ) : this(name, value.GetType(), expected) { }

		public ExpectedValueTypeException( TKey key, Type value, params Type[] expected ) : base(GetMessage(key, value, expected))
		{
			Key = key;
			Actual = value;
			Expected = expected;

			Data[nameof(Key)] = Key?.ToString();
			Data[nameof(Actual)] = Actual.FullName;
			Data[nameof(Expected)] = GetTypes(expected);
		}


		protected static string GetTypes( params Type[] expected ) =>
			expected.Aggregate("", ( current, item ) => current + @$"""{item.FullName}"", ");

		protected static string GetMessage( TKey key, Type actual, params Type[] expected ) =>
			@$"For the key ""{key}"", the value passed needs to be of following types: [ {GetTypes(expected)} ] but got ""{actual.FullName}"" ";
	}



	public class ExpectedValueTypeException : ExpectedValueTypeException<string> // Jakar.Api.Exceptions.Networking.HeaderException
	{
		public ExpectedValueTypeException( string key, object value, params Type[] expected ) : this(key, value.GetType(), expected) { }

		public ExpectedValueTypeException( string key, Type value, params Type[] expected ) : base(key, value, expected) { }
	}
}
