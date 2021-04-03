// unset

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	[TypeConversion(typeof(double))]
	public class NullableFontSizeConverter : FontSizeConverter // IExtendedTypeConverter 
	{
		public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType == typeof(string);
		public override object? ConvertFromInvariantString( string? value ) => Convert(value);

		public double? Convert( string? value ) =>
			value switch
			{
				null => default,
				_    => (double) base.ConvertFromInvariantString(value)
			};

		public string? ConvertToInvariantString( object? value ) => value?.ToString();
	}
}
