using System;
using System.Globalization;
using System.IO;
using Jakar.Extensions.Extensions;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	public class BytesToImageConverter : TypeConverter, IValueConverter, IExtendedTypeConverter
	{
		public ImageSource? Convert( object? value ) =>
			value switch
			{
				null                                => null,
				byte[] bytes                        => Convert(bytes),
				Memory<byte> readOnlyMemory         => Convert(readOnlyMemory.ToArray()),
				ReadOnlyMemory<byte> readOnlyMemory => Convert(readOnlyMemory.ToArray()),
				_                                   => null
			};

		public ImageSource? Convert( byte[]? value ) =>
			value switch
			{
				null => null,
				_    => ImageSource.FromStream(() => new MemoryStream(value))
			};


		public object? ConvertFrom( CultureInfo            culture, object?          value, IServiceProvider serviceProvider ) => Convert(value?.ToString());
		public object? ConvertFromInvariantString( string? value,   IServiceProvider serviceProvider ) => Convert(value);

		public override object? ConvertFromInvariantString( string? value ) => Convert(value?.FromBase64String());


		public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) => CanConvertFrom(value?.GetType())
																												? Convert(value)
																												: null;

		public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) => throw new NotImplementedException();


		public override bool CanConvertFrom( Type? value ) => value != null && CheckTypes(value);
		protected       bool CheckTypes( Type      value ) => typeof(byte[]) == value || typeof(Memory<byte>) == value || typeof(ReadOnlyMemory<byte>) == value;

		// <converters:BytesToImageConverter x:Key="BytesToImage" />
		// <Image Source="{Binding Image, Converter={StaticResource BytesToImage}}" Aspect="AspectFill" />
	}
}
