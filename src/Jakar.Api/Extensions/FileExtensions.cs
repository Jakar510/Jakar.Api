// unset

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jakar.Extensions.FileSystemExtensions;
using Jakar.Extensions.Models;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	// ReSharper disable once UnusedType.Global
	public static class FileExtensions
	{
		public static FileInfo ToFileInfo( this MediaFile file ) => new(file.Path ?? throw new ArgumentNullException(nameof(file)));


		public static async Task<LocalFile?> Pick( PickOptions? options = null )
		{
			FileResult? result = await FilePicker.PickAsync(options);

			return new LocalFile(result.FullPath);
		}

		public static async Task<IEnumerable<LocalFile>?> PickMultiple( PickOptions? options = null )
		{
			IEnumerable<FileResult>? items = await FilePicker.PickMultipleAsync(options);

			return items?.Select(item => new LocalFile(item.FullPath));
		}
	}
}
