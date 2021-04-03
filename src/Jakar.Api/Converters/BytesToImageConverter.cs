using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters
{
	public class BytesToImageConverter : IValueConverter
	{
		protected bool CanConvert( Type value ) => value != null && typeof(byte[]) == value;

		public object? Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return CanConvert(value.GetType())
					   ? ImageSource.FromStream(() => new MemoryStream((byte[]) value ?? throw new NullReferenceException(nameof(value))))
					   : null;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) => throw new NotImplementedException();

		// <converters:BytesToImageConverter x:Key="BytesToImage" />
		// <Image Source="{Binding Image, Converter={StaticResource BytesToImage}}" Aspect="AspectFill" />
	}
}
