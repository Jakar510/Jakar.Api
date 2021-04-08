
#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public interface IAppSettings
	{
		bool SendCrashes { get; set; }
		string? ScreenShotAddress { get; set; }
		object? CurrentViewPage { get; set; }
		string? AppName { get; set; }
		bool CrashDataPending { get; set; }
	}

	
	public class AppSettings : IAppSettings
	{
		public bool SendCrashes { get; set; }

		public string? ScreenShotAddress { get; set; }

		public virtual object? CurrentViewPage { get; set; }

		public string? AppName { get; set; }

		public bool CrashDataPending { get; set; }

		public static Version AppVersion { get; } = Version.Parse(DeviceInfo.FullVersion);
	}
}
