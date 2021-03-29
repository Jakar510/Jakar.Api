// unset

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;

namespace Jakar.Api.Models
{
	[Serializable]
	public class LocalFile : IDisposable
	{
		public FileInfo Info { get; }
		public string FileName { get; }
		public MimeType Mime { get; private set; }
		public string Path => Info.FullName;
		public bool Exists => File.Exists(Path);
		public void Encrypt() => File.Encrypt(Path);
		public void Decrypt() => File.Decrypt(Path);


		public LocalFile( string path ) : this(path, System.IO.Path.GetFileName(path)) { }
		public LocalFile( FileInfo path ) : this(path ?? throw new ArgumentNullException(nameof(path)), System.IO.Path.GetFileName(path.FullName)) { }
		public LocalFile( string path, string name ) : this(new FileInfo(path), name) { }
		public LocalFile( FileInfo info, string name )
		{
			Info = info;
			FileName = name;
		}

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
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose( bool remove )
		{
			if ( string.IsNullOrWhiteSpace(Path) ) return;

			if ( remove && File.Exists(Path) )
				File.Delete(Path);
		}

		public static implicit operator LocalFile( FileInfo info ) => new(info);
		public static LocalFile FromFileInfo( FileInfo info ) => new(info);
	}


	// ReSharper disable once ClassNeverInstantiated.Global
}