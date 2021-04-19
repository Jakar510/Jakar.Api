using System;
using System.Globalization;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	public class ObjectToStringConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
	{
		public override bool CanConvertFrom( Type? sourceType ) => true;

		public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => value?.ToString();
		public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => value;


		public object? ConvertFrom( CultureInfo culture, object? value, IServiceProvider serviceProvider ) => value?.ToString();
		public object? ConvertFromInvariantString( string? value, IServiceProvider serviceProvider ) => value;

		public override object? ConvertFromInvariantString( string? value ) => value;
	}
}
