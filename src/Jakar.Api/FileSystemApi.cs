using Jakar.Extensions;
using Jakar.Extensions.Models;
using Xamarin.Essentials;


#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public class FileSystemApi : BaseFileSystemApi
	{
		protected override string _AppDataDirectory => FileSystem.AppDataDirectory;
		protected override string _CacheDirectory   => FileSystem.CacheDirectory;

		public FileSystemApi() : base() { }
	}
}
