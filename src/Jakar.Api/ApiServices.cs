﻿using System;
using Jakar.Api.ResourceManager;
using Jakar.Extensions;
using Jakar.Extensions.Interfaces;
using Xamarin.Essentials;


#pragma warning disable 1591
#nullable enable
namespace Jakar.Api
{
	public abstract class ApiServices<TDebug, TPrompts, TAccounts, TUser, TActiveUser, TAppSettings, TResourceManager, TLanguage> where TAccounts : AccountManager<TUser, TActiveUser>, new()
																																  where TUser : class, IUser
																																  where TActiveUser : class, ICurrentUser<TUser>, new()
																																  where TAppSettings : IAppSettings, new()
																																  where TPrompts : Prompts, new()
																																  where TDebug : Debug, new()
																																  where TLanguage : LanguageApi, new()
																																  where TResourceManager : ResourceDictionaryManager, new()
	{
		public TDebug           Debug      { get; } = new();
		public TAccounts        Accounts   { get; } = new();
		public TPrompts         Prompts    { get; } = new();
		public TAppSettings     Settings   { get; } = new();
		public TResourceManager Resources  { get; } = new();
		public LocationManager  Location   { get; } = new();
		public BarometerReader  Barometer  { get; } = new();
		public TLanguage        Language   { get; } = new();
		public Commands         Loading    { get; }
		public FileSystemApi    FileSystem { get; } = new();


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

			Loading = new Commands(Prompts);
		}
	}
}
