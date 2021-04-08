using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.MarkupExtensions
{
	[ContentProperty(nameof(Source))]
	public abstract class ImageResourceExtension : IMarkupExtension
	{
		public string? Source { get; set; }

		protected abstract string GetPath( string fileName ); // Path.to.file.in.assembly

		public object? ProvideValue( IServiceProvider serviceProvider )
		{
			if ( Source is null ) { return null; }

			// Do your translation lookup here, using whatever method you require
			ImageSource imageSource = ImageSource.FromResource(GetPath(Source), typeof(ImageResourceExtension).GetTypeInfo().Assembly);

			return imageSource;
		}
	}
}
