// unset

using System;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class AppSettings
	{
		public static AppSettings Current => _Service.Value;
		private static Lazy<AppSettings> _Service { get; } = new(Create, false);
		private static AppSettings Create() => new();


		public bool SendCrashes { get; set; }

		public object? CurrentViewPage { get; set; }

		public string? ScreenShotAddress { get; set; }

		public string? AppName { get; set; }

		public long Version { get; set; }
		public static bool CrashDataPending { get; set; }
	}
}
