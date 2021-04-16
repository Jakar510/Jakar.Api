using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Extensions;
using Jakar.Api.Http;
using Jakar.Api.Statics;
using Newtonsoft.Json;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	public class FileData : IDisposable, IAsyncDisposable
	{
		public LocalFile LocalFile { get; }
		protected string _Path => LocalFile.FullPath;


		protected string? _data;

		protected ReadOnlyMemory<byte>? _payload;

		public string? Data
		{
			get => _data;
			init => _data = value;
		}

		public ReadOnlyMemory<byte>? Payload
		{
			get => _payload;
			init => _payload = value;
		}


		public FileData( string path ) : this(new LocalFile(path)) { }
		public FileData( FileInfo path ) : this(new LocalFile(path)) { }
		public FileData( FileBase path ) : this(new LocalFile(path)) { }
		public FileData( LocalFile path ) => LocalFile = path;


		public static implicit operator FileData( string info ) => new(info);
		public static implicit operator FileData( FileInfo info ) => new(info);
		public static implicit operator FileData( LocalFile info ) => new(info);
		public static implicit operator FileData( FileBase info ) => new(info);
		public static implicit operator FileData( Uri info ) => new(new LocalFile(info));


	#region Read

		public async Task<T?> ReadFromFileAsync<T>() where T : class
		{
			if ( await ReadFromFileAsync().ConfigureAwait(true) )
			{
				return _data is null
						   ? default
						   : JsonConvert.DeserializeObject<T>(_data);
			}

			return default;
		}

		public async Task<bool> ReadFromFileAsync()
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) || !LocalFile.Exists ) { return false; }

			using var stream = new StreamReader(path, Encoding.UTF8);
			_data = await stream.ReadToEndAsync().ConfigureAwait(true);

			return true;
		}

		public bool ReadFromFile()
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) || !LocalFile.Exists ) { return false; }

			using var stream = new StreamReader(path, Encoding.UTF8);
			_data = stream.ReadToEnd();
			return true;
		}


		public async Task<Stream> OpenReadAsync() => await LocalFile.Result.OpenReadAsync().ConfigureAwait(true);


		public async Task<bool> RawReadFromFileAsync()
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) || !LocalFile.Exists ) { return false; }

			await using FileStream file = File.OpenRead(path);
			await using var stream = new MemoryStream();
			await file.CopyToAsync(stream).ConfigureAwait(true);
			_payload = stream.ToArray();

			return true;
		}

		public bool RawReadFromFile()
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) || !LocalFile.Exists ) { return false; }

			using FileStream file = File.OpenRead(path);
			using var stream = new MemoryStream();
			file.CopyTo(stream);
			_payload = stream.ToArray();

			return true;
		}

	#endregion


	#region Write

		public async Task<bool> WriteToFileAsync()
		{
			if ( _payload is not null )
			{
				await WriteToFileAsync(_payload.Value.ToArray()).ConfigureAwait(true);
				return true;
			}

			if ( string.IsNullOrWhiteSpace(_data) ) { return false; }

			await WriteToFileAsync(_data).ConfigureAwait(true);
			return true;
		}

		public async Task WriteToFileAsync( object jsonSerializablePayload )
		{
			string json = jsonSerializablePayload.ToPrettyJson();
			await WriteToFileAsync(json).ConfigureAwait(true);
		}

		public async Task WriteToFileAsync( string data )
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) ) { throw new NullReferenceException(nameof(path)); }

			if ( string.IsNullOrWhiteSpace(data) ) throw new ArgumentNullException(nameof(data));

			await using FileStream file = File.Create(path); //https://stackoverflow.com/a/11541330/9530917
			await using var writer = new StreamWriter(file, Encoding.UTF8);
			await writer.WriteAsync(data).ConfigureAwait(true);
		}

		public async Task WriteToFileAsync( byte[] payload )
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) ) { throw new NullReferenceException(nameof(path)); }

			if ( payload is null ) throw new ArgumentNullException(nameof(payload));

			if ( payload.Length == 0 ) throw new ArgumentException(@"payload.Length == 0", nameof(payload));


			await using FileStream file = File.Create(path);
			await file.WriteAsync(payload, 0, payload.Length).ConfigureAwait(true);
		}

		public async Task WriteToFileAsync( Stream stream )
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) ) { throw new NullReferenceException(nameof(path)); }

			if ( stream is null ) throw new ArgumentNullException(nameof(stream));

			await using var memory = new MemoryStream();
			await stream.CopyToAsync(memory).ConfigureAwait(true);
			byte[] payload = memory.GetBuffer();
			await WriteToFileAsync(payload).ConfigureAwait(true);
		}

	#endregion


	#region Streams

		public FileStream? GetStream( in FileAccess access )
		{
			string path = _Path;

			if ( string.IsNullOrWhiteSpace(path) || !LocalFile.Exists ) { return null; }

			return File.Open(path, FileMode.OpenOrCreate, access);
		}

		public async Task<FileStream?> GetWriteStreamAsync( FileAccess access ) => await Task.FromResult(GetStream(access));

	#endregion


		public void Dispose() { LocalFile.Dispose(); }

		public async ValueTask DisposeAsync() { await LocalFile.DisposeAsync(); }


	#region Static

		public static async Task<FileData> SaveFileAsync( string path, Uri uri, CancellationToken token )
		{
			if ( uri.IsFile ) { return new FileData(uri); }

			var req = WebRequest.Create(uri);
			req.Method = "GET";
			req.ContentType = MimeTypeNames.Application.URL_ENCODED_CONTENT;

			using WebResponse resp = await req.GetResponseAsync(token).ConfigureAwait(true);

			await using Stream? stream = resp.GetResponseStream();
			return await SaveFileAsync(path, stream ?? throw new NullReferenceException(nameof(stream))).ConfigureAwait(true);
		}

		public static async Task<FileData> SaveFileAsync( string path, Stream stream )
		{
			var file = new FileData(FileSystemApi.GetAppDataPath(path));
			await file.WriteToFileAsync(stream).ConfigureAwait(true);
			return file;
		}

		public static async Task<FileData> SaveFileAsync( string path, byte[] payload )
		{
			var file = new FileData(path);
			await file.WriteToFileAsync(payload).ConfigureAwait(true);
			return file;
		}

		public static FileData SaveFile( string path, string uri ) => SaveFile(path, new Uri(uri));

		public static FileData SaveFile( string path, Uri uri )
		{
			if ( uri.IsFile ) { return new FileData(uri); }

			using var client = new WebClient();

			client.DownloadFile(uri, path);
			return new FileData(path);
		}

	#endregion
	}
}
