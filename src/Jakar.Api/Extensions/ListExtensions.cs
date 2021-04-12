using System;
using System.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class ListExtensions
	{
		public static bool IsEqual<TValue>( this TValue value, TValue other ) => Equals(value, other);
		public static bool IsOneOf<TValue>( this TValue value, params TValue[] items ) => items.Any(other => value.IsEqual(other));
	}
}
