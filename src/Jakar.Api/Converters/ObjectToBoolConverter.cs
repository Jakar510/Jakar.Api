using System;
using System.Globalization;
using Jakar.Extensions.Extensions;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	public class ObjectToBoolConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
	{
		public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType.IsOneOf(typeof(bool),
																											typeof(int),
																											typeof(uint),
																											typeof(long),
																											typeof(ulong),
																											typeof(float),
																											typeof(double),
																											typeof(string));

		public virtual object Convert( object? value )
		{
			return value switch
				   {
					   null     => false,
					   bool b   => b,
					   int n    => n != 0,
					   uint n   => n != 0,
					   long n   => n != 0,
					   ulong n  => n != 0,
					   float n  => n != 0,
					   double n => n != 0,
					   string s => bool.TryParse(s, out bool result)
									   ? result
									   : !string.IsNullOrWhiteSpace(s),
					   _ => true
				   };
		}

		public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => Convert(value);

		public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => value?.ToString();


		public object ConvertFrom( CultureInfo culture, object? value, IServiceProvider serviceProvider ) => Convert(value);
		public object ConvertFromInvariantString( string? value, IServiceProvider serviceProvider ) => Convert(value);


		public override object ConvertFromInvariantString( string? value ) => Convert(value);
	}
}
