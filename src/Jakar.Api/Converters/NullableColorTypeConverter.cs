using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	[TypeConversion(typeof(Color?))]
	public class NullableColorTypeConverter : ColorTypeConverter // IExtendedTypeConverter 
	{
		public override bool CanConvertFrom( Type? sourceType ) => sourceType is null || sourceType == typeof(string);
		public override object? ConvertFromInvariantString( string? value ) => Convert(value);
		public Color? Convert( string? value ) =>
			string.IsNullOrWhiteSpace(value)
				? null
				: (Color) base.ConvertFromInvariantString(value);
	}
}