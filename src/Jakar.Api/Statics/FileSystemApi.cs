using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Models;
using Xamarin.Essentials;


#pragma warning disable 1591


namespace Jakar.Api.Statics
{
	public static class FileSystemApi
	{
		public static string AppStateFileName { get; } = GetCacheDataPath("AppState.json");
		public static string DebugFileName { get; } = GetCacheDataPath("debug.txt");
		public static string FeedBackFileName { get; } = GetCacheDataPath("feedback.json");
		public static string OutgoingFileName { get; } = GetCacheDataPath("Outgoing.json");
		public static string IncomingFileName { get; } = GetCacheDataPath("Incoming.json");

		public static string AccountsFileName { get; } = GetAppDataPath($"accounts.json");
		public static string ZipFileName { get; } = GetAppDataPath("AppData.zip");
		public static string ScreenShot { get; } = GetCacheDataPath("ScreenShot.png");


		public static string GetAppDataPath( string fileName ) => Path.Combine(FileSystem.AppDataDirectory, fileName);
		public static string GetCacheDataPath( string fileName ) => Path.Combine(FileSystem.CacheDirectory, fileName);


		public static void CreateZipCache()
		{
			if ( File.Exists(ZipFileName) )
				File.Delete(ZipFileName);

			ZipCache();
		}

		public static void ZipCache() => ZipCache(FileSystem.CacheDirectory);

		public static void ZipCache( string path ) => ZipFile.CreateFromDirectory(path,
																				  ZipFileName,
																				  CompressionLevel.Optimal,
																				  true,
																				  Encoding.UTF8);

		public static async Task<string> ZipAsync( params string[] args ) => await ZipAsync(args.ToList()).ConfigureAwait(true);
		public static async Task<string> ZipAsync( params FileData[] files ) => await ZipAsync(files.ToList()).ConfigureAwait(true);
		public static async Task<string> ZipAsync( params LocalFile[] args ) => await ZipAsync(args.ToList()).ConfigureAwait(true);
		public static async Task<string> ZipAsync( IEnumerable<string> files ) => await ZipAsync(files.Select(item => new FileData(item))).ConfigureAwait(true);
		public static async Task<string> ZipAsync( IEnumerable<LocalFile> args ) => await ZipAsync(args.Select(item => new FileData(item))).ConfigureAwait(true);

		public static async Task<string> ZipAsync( IEnumerable<FileData> args )
		{
			if ( args is null ) throw new ArgumentNullException(nameof(args));

			await using FileStream zipToOpen = File.Create(ZipFileName);
			using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

			foreach ( FileData file in args )
			{
				ZipArchiveEntry entry = archive.CreateEntry(file.LocalFile.FileName);
				await using var writer = new StreamWriter(entry.Open());

				if ( await file.ReadFromFileAsync().ConfigureAwait(true) ) { await writer.WriteAsync(file.Data).ConfigureAwait(true); }
			}

			return ZipFileName;
		}

		//
		// public async Task<string> ZipAsync( IEnumerable<LocalFile> args )
		// {
		// 	if ( args is null ) throw new ArgumentNullException(nameof(args));
		//
		// 	await using FileStream zipToOpen = File.Create(ZipFileName);
		// 	using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
		//
		// 	foreach ( LocalFile file in args )
		// 	{
		// 		ZipArchiveEntry entry = archive.CreateEntry(file.FileName);
		// 		await using var writer = new StreamWriter(entry.Open());
		// 		string? data = await file.ReadFromFileAsync().ConfigureAwait(true);
		// 		if ( data is null ) continue;
		//
		// 		await writer.WriteAsync(data).ConfigureAwait(true);
		// 	}
		//
		// 	return ZipFileName;
		// }


		public static async Task<FileData?> SaveFileAsync( string filename, Uri uri, CancellationToken token ) => await FileData.SaveFileAsync(GetCacheDataPath(filename), uri, token).ConfigureAwait(true);

		public static async Task<FileData?> SaveFileAsync( string filename, Stream stream ) => await FileData.SaveFileAsync(GetCacheDataPath(filename), stream).ConfigureAwait(true);

		public static async Task<FileData?> SaveFileAsync( string filename, byte[] payload ) => await FileData.SaveFileAsync(GetCacheDataPath(filename), payload).ConfigureAwait(true);

		public static FileData SaveFile( string filename, string link ) => FileData.SaveFile(filename, new Uri(link));

		public static FileData SaveFile( string filename, Uri link ) => FileData.SaveFile(GetCacheDataPath(filename), link);
	}
}
