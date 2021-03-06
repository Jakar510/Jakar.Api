using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Provider;
using Jakar.Api.Droid.Services;
using Jakar.Api.Interfaces;
using Jakar.Extensions.Interfaces;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
[assembly: Dependency(typeof(FileService))]

namespace Jakar.Api.Droid.Services
{
	public class FileService : IFileService
	{
		//private bool isExternalStorageWritable() => Android.OS.Environment.GetExternalStorageState(Android.OS.Environment.DirectoryDownloads) == Android.OS.Environment.MediaMounted;

		public Task<FileInfo> DownloadFile( Uri link, string fileName )
		{
			if ( link is null ) throw new ArgumentNullException(nameof(link));
			if ( string.IsNullOrWhiteSpace(fileName) ) throw new ArgumentNullException(nameof(fileName));

			using var client = new WebClient();

			Java.IO.File? root = BaseApplication.Instance.GetExternalFilesDir(MediaStore.Downloads.ContentType);
			if ( root is null ) throw new NullReferenceException(nameof(root));
			string path = Path.Combine(root.AbsolutePath, fileName);

			client.DownloadFile(link, path);

			return Task.FromResult(new FileInfo(path));
		}
	}
}
