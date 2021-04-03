// unset

using System;
using System.IO;
using System.Threading.Tasks;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	[Serializable]
	public class LocalFile : IDisposable, IAsyncDisposable
	{
		public bool IsTemporary { get; init; }
		public FileInfo Info { get; }
		public string FileName { get; }
		public MimeType Mime { get; private set; }
		public string Path => Info.FullName;
		public bool Exists => File.Exists(Path);

		protected static string FromUri( Uri uri )
		{
			if ( !uri.IsFile ) throw new ArgumentException("Uri is not a file.", nameof(uri));

			return uri.AbsolutePath;
		}

		public LocalFile( Uri path, bool temporary = false ) : this(FromUri(path), temporary) { }
		public LocalFile( string path, bool temporary = false ) : this(path, System.IO.Path.GetFileName(path), temporary) { }
		public LocalFile( string path, string name, bool temporary = false ) : this(new FileInfo(path), name, temporary) { }
		public LocalFile( FileInfo path, bool temporary = false ) : this(path ?? throw new ArgumentNullException(nameof(path)), System.IO.Path.GetFileName(path.FullName), temporary) { }

		public LocalFile( FileInfo info, string name, bool temporary = false )
		{
			IsTemporary = temporary;
			Info = info;
			FileName = name;
		}

		public static implicit operator LocalFile( string info ) => new(info);
		public static implicit operator LocalFile( FileInfo info ) => new(info);
		public static implicit operator LocalFile( Uri info ) => new(info);


		public void Encrypt() => File.Encrypt(Path);
		public void Decrypt() => File.Decrypt(Path);

		public Uri ToUri() => ToUri(MimeType.Unknown);

		public Uri ToUri( MimeType mime )
		{
			if ( string.IsNullOrWhiteSpace(Path) )
				throw new NullReferenceException(nameof(Path));

			Mime = mime;

			if ( !Path.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) )
				return new Uri($"{Mime.ToUriScheme()}://{Path}", UriKind.Absolute);

			string path = Path.Remove(0, 1);
			return new Uri($"{Mime.ToUriScheme()}://{path}", UriKind.Absolute);
		}


		public void Dispose()
		{
			Dispose(IsTemporary);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose( bool remove )
		{
			if ( string.IsNullOrWhiteSpace(Path) ) return;

			if ( remove && File.Exists(Path) )
				File.Delete(Path);
		}

		public ValueTask DisposeAsync()
		{
			Dispose();
			return new ValueTask(Task.CompletedTask);
		}
	}



	// ReSharper disable once ClassNeverInstantiated.Global
}
