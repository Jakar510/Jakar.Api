using System;
using Jakar.Api.ResourceManager;
using Jakar.Extensions;
using Jakar.Extensions.Interfaces;
using Jakar.Extensions.Languages;
using Jakar.Extensions.Models;


#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public abstract class ApiServices<TDebug,
									  TPrompts,
									  TAccounts,
									  TUser,
									  TAppSettings,
									  TFileSystem,
									  TLanguage,
									  TResourceManager,
									  TDeviceID,
									  TViewPage> where TAccounts : AccountManager<TUser>
												 where TUser : class, IUser, new()
												 where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
												 where TPrompts : Prompts<TDeviceID, TViewPage>, new()
												 where TDebug : Debug<TDeviceID, TViewPage>, new()
												 where TLanguage : LanguageApi, new()
												 where TFileSystem : BaseFileSystemApi, new()
												 where TResourceManager : ResourceDictionaryManager, new()
	{
		public TDebug                         Debug      { get; } = new();
		public TAccounts                      Accounts   { get; }
		public TPrompts                       Prompts    { get; } = new();
		public TAppSettings                   Settings   { get; } = new();
		public TResourceManager               Resources  { get; } = new();
		public LocationManager                Location   { get; } = new();
		public BarometerReader                Barometer  { get; } = new();
		public TLanguage                      Language   { get; } = new();
		public TFileSystem                    FileSystem { get; } = new();
		public Commands<TDeviceID, TViewPage> Loading    { get; }


		/// <summary>
		/// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
		/// </summary>
		/// <param name="app_center_id"></param>
		/// <param name="appCenterServices"></param>
		protected ApiServices( in string app_center_id, params Type[] appCenterServices )
		{
			Accounts = InstanceCreator.Create<TAccounts>(FileSystem);
			Prompts.Init(Debug);
			Prompts.Init(Settings);
			Debug.Init(FileSystem, Settings, app_center_id, appCenterServices);

			Loading = new Commands<TDeviceID, TViewPage>(Prompts);
		}
	}
}
