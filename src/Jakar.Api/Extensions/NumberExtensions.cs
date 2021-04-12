// unset

using System;
using Xamarin.Forms.Internals;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class NumberExtensions
	{
		public static float ToFloat( this double value ) => (float) value;
		public static int ToInt( this double value ) => (int) value;

		public static double ToDouble<TValue>( this TValue value ) where TValue : Enum => value.ToLong();
		public static float ToFloat<TValue>( this TValue value ) where TValue : Enum => value.ToLong();
		public static long ToLong<TValue>( this TValue value ) where TValue : Enum => Convert.ToInt64(value);
		public static int ToInt<TValue>( this TValue value ) where TValue : Enum => Convert.ToInt32(value);
	}
}
