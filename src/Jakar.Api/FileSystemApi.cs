using Xamarin.Essentials;


#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public class FileSystemApi : Jakar.Extensions.FileSystemApi
	{
		protected override string _AppDataDirectory => FileSystem.AppDataDirectory;
		protected override string _CacheDirectory => FileSystem.CacheDirectory;
	}
}
