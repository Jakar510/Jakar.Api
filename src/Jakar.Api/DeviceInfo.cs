namespace Jakar.Api
{
	#pragma warning disable 1591

	#nullable enable
	public static class DeviceInfo
	{
		internal static readonly string versionNumber = Xamarin.Essentials.AppInfo.VersionString;
		internal static readonly string buildNumber = Xamarin.Essentials.AppInfo.BuildString;
		public static readonly string _packageName = Xamarin.Essentials.AppInfo.PackageName;
		internal static readonly string deviceModel = Xamarin.Essentials.DeviceInfo.Model;
		internal static readonly string manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer;
		internal static readonly string version = Xamarin.Essentials.DeviceInfo.VersionString;
		internal static readonly string devicePlatform = Xamarin.Essentials.DeviceInfo.Platform.ToString();

		public static string FullVersion { get; } = $"{versionNumber}.{buildNumber}";
		public static string DeviceId { get; } = $"{manufacturer}  {deviceModel}: {devicePlatform} {version} | {versionNumber} [{buildNumber}]";
	}
}
