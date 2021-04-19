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


		public static int? ToNullableInt( this string? value, in int? defaultValue = null ) => string.IsNullOrWhiteSpace(value)
																								   ? null
																								   : int.TryParse(value, out int result)
																									   ? result
																									   : defaultValue;

		public static int? ToInt( this string? value, in int defaultValue ) => int.TryParse(value, out int result)
																				   ? result
																				   : defaultValue;


		public static long? ToNullableLong( this string? value, in long? defaultValue = null ) => string.IsNullOrWhiteSpace(value)
																									  ? null
																									  : long.TryParse(value, out long result)
																										  ? result
																										  : defaultValue;

		public static long? ToLong( this string? value, in long defaultValue ) => long.TryParse(value, out long result)
																					  ? result
																					  : defaultValue;


		public static float? ToNullableFloat( this string? value ) => string.IsNullOrWhiteSpace(value)
																		  ? null
																		  : value.ToFloat();

		public static float ToFloat( this string value ) => float.TryParse(value, out float d)
																? d
																: float.NaN;


		public static double? ToNullableDouble( this string? value ) => string.IsNullOrWhiteSpace(value)
																			? null
																			: value.ToDouble();

		public static double ToDouble( this string value ) => double.TryParse(value, out double d)
																  ? d
																  : double.NaN;


		public static double ToDouble<TValue>( this TValue value ) where TValue : Enum => value.ToLong();
		public static float ToFloat<TValue>( this TValue value ) where TValue : Enum => value.ToLong();
		public static long ToLong<TValue>( this TValue value ) where TValue : Enum => Convert.ToInt64(value);
		public static int ToInt<TValue>( this TValue value ) where TValue : Enum => Convert.ToInt32(value);
	}
}
