
#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class DeviceInfo
	{
		public static string VersionNumber => Xamarin.Essentials.AppInfo.VersionString;
		public static string BuildNumber => Xamarin.Essentials.AppInfo.BuildString;
		public static string PackageName => Xamarin.Essentials.AppInfo.PackageName;
		public static string DeviceModel => Xamarin.Essentials.DeviceInfo.Model;
		public static string Manufacturer => Xamarin.Essentials.DeviceInfo.Manufacturer;
		public static string Version => Xamarin.Essentials.DeviceInfo.VersionString;
		public static string DevicePlatform => Xamarin.Essentials.DeviceInfo.Platform.ToString();

		public static string FullVersion { get; } = $"{VersionNumber}.{BuildNumber}";
		public static string DeviceId { get; } = $"{Manufacturer}  {DeviceModel}: {DevicePlatform} {Version} | {VersionNumber} [{BuildNumber}]";
	}
}
