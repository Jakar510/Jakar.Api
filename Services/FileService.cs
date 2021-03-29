using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Foundation;
using TrueLogicMobile.API;
using TrueLogicMobile.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]
namespace TrueLogicMobile.iOS.Services
{
	public class FileService : Api.FileSystem.IFileService
	{
		public static string GetCacheDataPath( string fileName ) => Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, fileName);
		
		//public Task<FileInfo> DownloadFile( Uri link, string fileName )
		//{
		//	if ( link is null )
		//		throw new ArgumentNullException(nameof(link));

		//	if ( string.IsNullOrWhiteSpace(fileName) )
		//		throw new ArgumentNullException(nameof(fileName));

		//	using WebClient client = new WebClient();

		//	var manager = NSFileManager.DefaultManager;
		//	//UIDocumentPickerViewController
		//	//UIDocumentInteractionController

		//	var root = FileSystem.SharedPath;
		//	string path     = Path.Combine(root, fileName);

		//	client.DownloadFile(link, path);

		//	return Task.FromResult(new FileInfo(path));
		//}
		public Task<FileInfo> DownloadFile( Uri link, string fileName )
		{
			using var client = new WebClient();
			string path = GetCacheDataPath(fileName);
			client.DownloadFile(link, path);
			var info = new FileInfo(path);
			return Task.FromResult(info);
		}
	}
	public static class FileSystem
	{
		public static string SharedPath
		{
			get
			{
				string folder = SysMajorVersion > 7 ? 
					NSSearchPath.GetDirectories(NSSearchPathDirectory.SharedPublicDirectory, NSSearchPathDomain.User).First() : 
					Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				return folder;
			}
		}
		public static int SysMajorVersion
		{
			get
			{
				string value = UIDevice.CurrentDevice.SystemVersion.Split('.')[0];
				return int.TryParse(value, out int result) ? result : -1;
			}
		}
	}

}