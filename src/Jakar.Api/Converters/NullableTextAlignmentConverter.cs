using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	[TypeConversion(typeof(TextAlignment?))]
	public class NullableTextAlignmentConverter : TextAlignmentConverter
	{
		public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType == typeof(string);
		public override object? ConvertFromInvariantString( string? value ) => Convert(value);

		public TextAlignment? Convert( string? value )
		{
			if ( string.IsNullOrWhiteSpace(value) ) return null;
			return (TextAlignment) base.ConvertFromInvariantString(value);
		}

		public string? ConvertToInvariantString( object? value ) =>
			value switch
			{
				null                    => null,
				TextAlignment alignment => alignment.ToString(),
				_                       => value.ToString()
			};
	}
}
