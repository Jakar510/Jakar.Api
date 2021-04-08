// unset

using System;
using Jakar.Api.Interfaces;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class ApiServices
	{
		public static ApiServices Current { get; set; } = new();

		public bool SendCrashes { get; set; }

		public string? ScreenShotAddress { get; set; }

		public object? CurrentViewPage { get; set; }

		public string? AppName { get; set; }

		public bool CrashDataPending { get; set; }

		public static Version AppVersion { get; } = Version.Parse(DeviceInfo.FullVersion);
	}



	public abstract class ApiServices<TDebug, TPrompts, TAccounts, TUser, TActiveUser> : ApiServices where TAccounts : AccountManager<TUser, TActiveUser>, new()
																									 where TUser : class, IUser
																									 where TActiveUser : class, ICurrentUser<TUser>, new()
																									 where TPrompts : Prompts, new()
																									 where TDebug : Debug, new()
	{
		public new static ApiServices<TDebug, TPrompts, TAccounts, TUser, TActiveUser> Current
		{
			get => (ApiServices<TDebug, TPrompts, TAccounts, TUser, TActiveUser>) ApiServices.Current;
			set => ApiServices.Current = value;
		}

		// public static Language Current => _Service.Value;
		// private static Lazy<Language> _Service { get; } = new(Create, false);
		// private static Language Create() => new();

		public TDebug Debug { get; } = new();

		public TAccounts Accounts { get; } = new();

		public TPrompts Prompts { get; } = new();

		/// <summary>
		/// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
		/// </summary>
		/// <param name="app_center_id"></param>
		/// <param name="appCenterServices"></param>
		protected ApiServices( in string app_center_id, params Type[] appCenterServices )
		{
			Prompts.Init(Debug);


			Prompts.Init(this);

			Debug.Init(this, app_center_id, appCenterServices);
		}
	}
}
