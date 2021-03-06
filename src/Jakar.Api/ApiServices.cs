using System;
using Jakar.Api.ResourceManager;
using Jakar.Extensions;
using Jakar.Extensions.FileSystemExtensions;
using Jakar.Extensions.Interfaces;
using Jakar.Extensions.Languages;
using Jakar.Extensions.Models;


#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public abstract class ApiServices<TDebug,
									  TPrompts,
									  TAppSettings,
									  TFileSystem,
									  TLanguage,
									  TDeviceID,
									  TViewPage> where TDebug : Debug<TDeviceID, TViewPage>, new()
												 where TPrompts : Prompts<TDeviceID, TViewPage>, new()
												 where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
												 where TFileSystem : BaseFileSystemApi, new()
												 where TLanguage : LanguageApi, new()
	{
		public TDebug                         Debug      { get; } = new();
		public TPrompts                       Prompts    { get; } = new();
		public TAppSettings                   Settings   { get; } = new();
		public TLanguage                      Language   { get; } = new();
		public TFileSystem                    FileSystem { get; } = new();
		public LocationManager                Location   { get; } = new();
		public BarometerReader                Barometer  { get; } = new();
		public Commands<TDeviceID, TViewPage> Loading    { get; }


		/// <summary>
		/// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
		/// </summary>
		/// <param name="app_center_id"></param>
		/// <param name="appCenterServices"></param>
		protected ApiServices( in string app_center_id, params Type[] appCenterServices )
		{
			Prompts.Init(Debug);
			Prompts.Init(Settings);
			Debug.Init(FileSystem, Settings, app_center_id, appCenterServices);

			Loading = new Commands<TDeviceID, TViewPage>(Prompts);
		}
	}



	public abstract class ApiServices<TDebug,
									  TPrompts,
									  TAppSettings,
									  TFileSystem,
									  TLanguage,
									  TResourceManager,
									  TDeviceID,
									  TViewPage> :
		ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TDeviceID, TViewPage> where TDebug : Debug<TDeviceID, TViewPage>, new()
																								  where TPrompts : Prompts<TDeviceID, TViewPage>, new()
																								  where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
																								  where TFileSystem : BaseFileSystemApi, new()
																								  where TLanguage : LanguageApi, new()
																								  where TResourceManager : BaseResourceDictionaryManager, new()
	{
		public TResourceManager Resources { get; } = new();


		/// <summary>
		/// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
		/// </summary>
		/// <param name="app_center_id"></param>
		/// <param name="appCenterServices"></param>
		protected ApiServices( in string app_center_id, params Type[] appCenterServices ) : base(app_center_id, appCenterServices) { }
	}



	public abstract class ApiServices<TDebug,
									  TPrompts,
									  TAppSettings,
									  TAccounts,
									  TUser,
									  TFileSystem,
									  TLanguage,
									  TResourceManager,
									  TDeviceID,
									  TViewPage> :
		ApiServices<TDebug, TPrompts, TAppSettings, TFileSystem, TLanguage, TResourceManager, TDeviceID, TViewPage> where TDebug : Debug<TDeviceID, TViewPage>, new()
																													where TPrompts : Prompts<TDeviceID, TViewPage>, new()
																													where TAppSettings : IAppSettings<TDeviceID, TViewPage>, new()
																													where TAccounts : AccountManager<TUser>
																													where TUser : class, IUser<TUser>, new()
																													where TFileSystem : BaseFileSystemApi, new()
																													where TLanguage : LanguageApi, new()
																													where TResourceManager : BaseResourceDictionaryManager, new()
	{
		public TAccounts Accounts { get; }


		/// <summary>
		/// appCenterServices: pass in the types you want to initialize, for example:  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes)
		/// </summary>
		/// <param name="app_center_id"></param>
		/// <param name="appCenterServices"></param>
		protected ApiServices( in string app_center_id, params Type[] appCenterServices ) : base(app_center_id, appCenterServices) => Accounts = InstanceCreator<TAccounts>.Create(FileSystem);
	}
}
