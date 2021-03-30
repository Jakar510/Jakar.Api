using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Extensions;
using Jakar.Api.Models;
using Newtonsoft.Json;
#pragma warning disable 1591

namespace Jakar.Api
{
	public class FileSystem
	{
		public static FileSystem Current => _Service.Value;
		private static Lazy<FileSystem> _Service { get; } = new(Create, false);
		private static FileSystem Create() => new();


		public static string AppStateFileName { get; } = GetCacheDataPath("AppState.json");
		public static string DebugFileName { get; } = GetCacheDataPath("debug.txt");
		public static string FeedBackFileName { get; } = GetCacheDataPath("feedback.json");
		public static string OutgoingFileName { get; } = GetCacheDataPath("Outgoing.json");
		public static string IncomingFileName { get; } = GetCacheDataPath("Incoming.json");

		public static string AccountsFileName { get; } = GetAppDataPath($"accounts.json");
		public static string ZipFileName { get; } = GetAppDataPath($"{AppSettings.Current.AppName}.zip");
		public static string ScreenShot { get; } = GetCacheDataPath($"{AppSettings.Current.AppName}.png");


		public static string GetAppDataPath( string fileName ) => Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, fileName);
		public static string GetCacheDataPath( string fileName ) => Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, fileName);


		public async Task WriteToFileAsync( string path, object jsonSerializablePayload )
		{
			string json = Debug.Current.PrettyJson(jsonSerializablePayload);
			await WriteToFileAsync(path, json).ConfigureAwait(true);
		}
		public async Task WriteToFileAsync( string path, string prettyJson )
		{
			if ( string.IsNullOrWhiteSpace(prettyJson) ) throw new ArgumentNullException(nameof(prettyJson));
			if ( string.IsNullOrWhiteSpace(path) ) throw new ArgumentNullException(nameof(path));

			await using FileStream file = File.Create(path); //https://stackoverflow.com/a/11541330/9530917
			await using var writer = new StreamWriter(file, Encoding.UTF8);
			await writer.WriteAsync(prettyJson).ConfigureAwait(true);
		}
		public async Task WriteToFileAsync( string path, byte[] payload )
		{
			if ( payload is null ) throw new ArgumentNullException(nameof(payload));
			if ( payload.Length == 0 ) throw new ArgumentException(@"payload.Length == 0", nameof(payload));
			if ( string.IsNullOrWhiteSpace(path) ) throw new ArgumentNullException(nameof(path));

			await using FileStream file = File.Create(path);
			await file.WriteAsync(payload, 0, payload.Length).ConfigureAwait(true);
		}
		public async Task WriteToFileAsync( string path, Stream stream )
		{
			if ( stream is null ) throw new ArgumentNullException(nameof(stream));
			if ( string.IsNullOrWhiteSpace(path) ) throw new ArgumentNullException(nameof(path));

			await using var memory = new MemoryStream();
			await stream.CopyToAsync(memory).ConfigureAwait(true);
			byte[] payload = memory.GetBuffer();
			await WriteToFileAsync(path, payload).ConfigureAwait(true);
		}


		public async Task<T?> ReadFromFileAsync<T>( string fileName ) where T : class
		{
			try
			{
				string? data = await ReadFromFileAsync(fileName).ConfigureAwait(true);
				return data is null ? default : JsonConvert.DeserializeObject<T>(data);
			}
			catch ( Exception e )
			{
				await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true);
				return default;
			}
		}
		public async Task<string?> ReadFromFileAsync( string filePath )
		{
			try
			{
				if ( string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) )
					return null;

				using var stream = new StreamReader(filePath, Encoding.UTF8);
				return await stream.ReadToEndAsync().ConfigureAwait(true);
			}
			catch ( Exception e )
			{
				await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true);
				return null;
			}
		}
		public async Task<byte[]?> RawReadFromFileAsync( string path )
		{
			try
			{
				if ( string.IsNullOrWhiteSpace(path) || !File.Exists(path) )
					return null;

				await using var file = File.OpenRead(path);
				await using var stream = new MemoryStream();
				await file.CopyToAsync(stream).ConfigureAwait(true);
				return stream.ToArray();
			}
			catch ( Exception e )
			{
				await Debug.Current.HandleExceptionAsync(e).ConfigureAwait(true);
				return null;
			}
		}
		public string? ReadFromFile( string filePath )
		{
			try
			{
				if ( string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath) )
					return null;

				using var stream = new StreamReader(filePath, Encoding.UTF8);
				return stream.ReadToEnd();
			}
			catch ( Exception e )
			{
				Debug.Current.HandleException(e);
				return null;
			}
		}


		public async Task<string> ZipAsync( params FileData[] args ) => await ZipAsync(args.ToList()).ConfigureAwait(true);
		public async Task<string> ZipAsync( IList<FileData> args )
		{
			if ( args is null )
				throw new ArgumentNullException(nameof(args));

			await using FileStream zipToOpen = File.Create(ZipFileName);
			using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

			foreach ( FileData data in args )
			{
				ZipArchiveEntry entry = archive.CreateEntry(data.FileName);
				await using var writer = new StreamWriter(entry.Open());
				await writer.WriteAsync(data.Data).ConfigureAwait(true);
			}

			return ZipFileName;
		}

		public async Task<string> ZipAsync( params LocalFile[] args ) => await ZipAsync(args.ToList()).ConfigureAwait(true);
		public async Task<string> ZipAsync( IList<LocalFile> args )
		{
			if ( args is null ) throw new ArgumentNullException(nameof(args));

			await using FileStream zipToOpen = File.Create(ZipFileName);
			using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

			foreach ( LocalFile file in args )
			{
				ZipArchiveEntry entry = archive.CreateEntry(file.FileName);
				await using var writer = new StreamWriter(entry.Open());
				string? data = await ReadFromFileAsync(file.Path).ConfigureAwait(true);
				if ( data is null ) continue;

				await writer.WriteAsync(data).ConfigureAwait(true);
			}

			return ZipFileName;
		}

		public async Task<string> ZipAsync( params string[] args ) => await ZipAsync(args.ToList()).ConfigureAwait(true);
		public async Task<string> ZipAsync( IList<string> names )
		{
			if ( names is null )
				throw new ArgumentNullException(nameof(names));

			await using var zipToOpen = new FileStream(ZipFileName, FileMode.Open);
			using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

			foreach ( string file in names )
			{
				ZipArchiveEntry entry = archive.CreateEntry(file);
				await using var writer = new StreamWriter(entry.Open());

				string? data = await ReadFromFileAsync(file).ConfigureAwait(true);
				if ( data is null )
					continue;

				await writer.WriteAsync(data).ConfigureAwait(true);
			}

			return ZipFileName;
		}


		public void CreateZipCache()
		{
			if ( File.Exists(ZipFileName) )
				File.Delete(ZipFileName);

			ZipCache();
		}
		public void ZipCache() => ZipCache(Xamarin.Essentials.FileSystem.CacheDirectory);
		public void ZipCache( string path ) => ZipFile.CreateFromDirectory(path, ZipFileName, CompressionLevel.Optimal, true, Encoding.UTF8);


		public async Task<LocalFile> SaveFileAsync( string filename, Uri uri, CancellationToken token )
		{
			if ( filename is null ) throw new ArgumentNullException(nameof(filename));
			if ( uri is null ) throw new ArgumentNullException(nameof(uri));

			var req = WebRequest.Create(uri);
			req.Method = "GET";
			req.ContentType = MediaTypeNames.URL_ENCODED_CONTENT;
			req.Headers[HttpRequestHeader.ContentEncoding] = Encoding.UTF8.ToString();

			using WebResponse resp = await req.GetResponseAsync(token).ConfigureAwait(true);

			await using Stream? stream = resp.GetResponseStream();
			return await SaveFileAsync(filename, stream ?? throw new NullReferenceException(nameof(stream))).ConfigureAwait(true);
		}
		public async Task<LocalFile> SaveFileAsync( string filename, Stream stream )
		{
			if ( filename is null ) throw new ArgumentNullException(nameof(filename));
			if ( stream is null ) throw new ArgumentNullException(nameof(stream));

			string path = GetAppDataPath(filename);
			await WriteToFileAsync(path, stream).ConfigureAwait(true);
			return new LocalFile(path);
		}
		public async Task<LocalFile> SaveFileAsync( string filename, byte[] payload )
		{
			if ( filename is null ) throw new ArgumentNullException(nameof(filename));
			if ( payload is null ) throw new ArgumentNullException(nameof(payload));

			string path = GetCacheDataPath(filename);
			await WriteToFileAsync(path, payload).ConfigureAwait(true);
			return new LocalFile(path);
		}
		public LocalFile SaveFile( string filename, string link ) => SaveFile(filename, new Uri(link));
		public LocalFile SaveFile( string filename, Uri link )
		{
			using var client = new WebClient();
			string path = GetCacheDataPath(filename);
			//string path = DependencyService.Get<Api.FileSystem.IFileLocation>().GetDownloadsPath(filename);
			Debug.Current.PrintMessage(path);
			client.DownloadFile(link, path);
			return new LocalFile(path);
		}
	}
}