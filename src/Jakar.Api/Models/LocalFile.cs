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
	public interface ITempFile
	{
		internal bool IsTemporary { get; set; }
	}



	public static class TempFileExtensions
	{
		public static TItem SetTemporary<TItem>( this TItem file ) where TItem : ITempFile
		{
			file.IsTemporary = true;
			return file;
		}

		public static TItem SetNormal<TItem>( this TItem file ) where TItem : ITempFile
		{
			file.IsTemporary = false;
			return file;
		}

		public static bool IsTempFile( this ITempFile file ) => file.IsTemporary;
	}



	[Serializable]
	public class LocalFile : IDisposable, IAsyncDisposable, ITempFile
	{
		bool ITempFile.IsTemporary { get; set; }
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

		public LocalFile( Uri path ) : this(FromUri(path)) { }
		public LocalFile( string path ) : this(new FileInfo(path)) { }

		// ReSharper disable once SuggestBaseTypeForParameter
		public LocalFile( FileInfo file ) : this(new FileResult(file.FullName)) { }

		public LocalFile( FileBase info )
		{
			this.SetNormal();
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
			Dispose(this.IsTempFile());
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
