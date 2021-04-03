namespace Jakar.Api
{
	#pragma warning disable 1591

	#nullable enable
	public static class DeviceInfo
	{
		internal static readonly string VersionNumber = Xamarin.Essentials.AppInfo.VersionString;
		internal static readonly string BuildNumber = Xamarin.Essentials.AppInfo.BuildString;
		public static readonly string PackageName = Xamarin.Essentials.AppInfo.PackageName;
		internal static readonly string DeviceModel = Xamarin.Essentials.DeviceInfo.Model;
		internal static readonly string Manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer;
		internal static readonly string Version = Xamarin.Essentials.DeviceInfo.VersionString;
		internal static readonly string DevicePlatform = Xamarin.Essentials.DeviceInfo.Platform.ToString();

		public static string FullVersion { get; } = $"{VersionNumber}.{BuildNumber}";
		public static string DeviceId { get; } = $"{Manufacturer}  {DeviceModel}: {DevicePlatform} {Version} | {VersionNumber} [{BuildNumber}]";
	}
}
