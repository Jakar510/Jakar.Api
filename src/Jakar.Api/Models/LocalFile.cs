// unset

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	[Serializable]
	public class LocalFile : IDisposable, IAsyncDisposable
	{
		public bool IsTemporary { get; init; }
		public FileBase Result { get; }

		public FileInfo Info => new(FullPath);
		public bool Exists => File.Exists(FullPath);

		public string FullPath => Result.FullPath;
		public string FileName => Result.FileName;
		public string Extension => Path.GetExtension(Result.FileName);
		public MimeType Mime => Extension.FromExtension();
		public string ContentType => Result.ContentType;


		protected static FileInfo FromUri( Uri uri )
		{
			if ( !uri.IsFile ) throw new ArgumentException("Uri is not a file.", nameof(uri));

			return new FileInfo(uri.AbsolutePath);
		}

		public LocalFile( Uri path, bool temporary = false ) : this(FromUri(path), temporary) { }
		public LocalFile( string path, bool temporary = false ) : this(new FileInfo(path), temporary) { }
		public LocalFile( FileSystemInfo file, bool temporary = false ) : this(new FileResult(file.FullName), temporary) { }

		public LocalFile( FileBase info, bool temporary = false )
		{
			IsTemporary = temporary;
			Result = info;
		}

		public static async Task<LocalFile?> Pick( PickOptions? options = null )
		{
			FileResult? result = await FilePicker.PickAsync(options);

			return new LocalFile(result);
		}

		public static async Task<IEnumerable<LocalFile>?> PickMultiple( PickOptions? options = null )
		{
			IEnumerable<FileResult>? items = await FilePicker.PickMultipleAsync(options);

			return items?.Select(item => new LocalFile(item));
		}

		public static implicit operator LocalFile( string info ) => new(info);
		public static implicit operator LocalFile( FileInfo info ) => new(info);
		public static implicit operator LocalFile( FileBase info ) => new(info);
		public static implicit operator LocalFile( Uri info ) => new(info);


		public void Encrypt() => File.Encrypt(FullPath);
		public void Decrypt() => File.Decrypt(FullPath);


		public Uri ToUri( MimeType? mime = null )
		{
			if ( string.IsNullOrWhiteSpace(FullPath) ) { throw new NullReferenceException(nameof(FullPath)); }

			MimeType type = mime ?? Mime;

			if ( !FullPath.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ) { return new Uri($"{type.ToUriScheme()}://{FullPath}", UriKind.Absolute); }

			string path = FullPath.Remove(0, 1);
			return new Uri($"{type.ToUriScheme()}://{path}", UriKind.Absolute);
		}


		public void Dispose()
		{
			Dispose(IsTemporary);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose( bool remove )
		{
			if ( string.IsNullOrWhiteSpace(FullPath) ) return;

			if ( remove && Exists ) { File.Delete(FullPath); }
		}

		public ValueTask DisposeAsync()
		{
			Dispose();
			return new ValueTask(Task.CompletedTask);
		}
	}



	// ReSharper disable once ClassNeverInstantiated.Global
}
