// unset

using System;
using System.IO;
using Plugin.Media.Abstractions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	// ReSharper disable once UnusedType.Global
	public static class FileExtensions
	{
		public static FileInfo ToFileInfo( this MediaFile file ) => new(file.Path ?? throw new ArgumentNullException(nameof(file)));
	}
}
