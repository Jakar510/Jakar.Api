
#pragma warning disable 1591
#nullable enable
namespace Jakar.Api.Interfaces
{
	public interface IAppSettings
	{
		bool SendCrashes { get; set; }
		string? ScreenShotAddress { get; set; }
		object? CurrentViewPage { get; set; }
		string? AppName { get; set; }
		bool CrashDataPending { get; set; }
	}
}
