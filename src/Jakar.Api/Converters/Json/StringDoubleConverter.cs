using System;
using System.Globalization;
using Newtonsoft.Json;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters.Json
{
	public sealed class StringDoubleConverter : JsonConverter, IValueConverter, IExtendedTypeConverter
	{
		public override bool CanWrite => false;
		public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotSupportedException();

		public override object? ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) => Convert(reader.Value?.ToString());

		public double? Convert( string? value ) =>
			string.IsNullOrWhiteSpace(value)
				? null
				: double.TryParse(value, out double d)
					? d
					: double.NaN;

		public override bool CanConvert( Type objectType ) => objectType == typeof(string) || objectType == null;


		public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => Convert(value?.ToString());

		public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
		{
			if ( value is double number ) { return number.ToString(culture); }

			return value?.ToString();
		}

		public object? ConvertFrom( CultureInfo culture, object? value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
		public object? ConvertFromInvariantString( string? value, IServiceProvider serviceProvider ) => Convert(value);
	}
}
