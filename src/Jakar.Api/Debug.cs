using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jakar.Api.Interfaces;
using Jakar.Api.Statics;
using Jakar.Extensions;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.FileSystemExtensions;
using Jakar.Extensions.General;
using Jakar.Extensions.Interfaces;
using Jakar.Extensions.Languages;
using Jakar.Extensions.Models;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class Debug<TDeviceID, TViewPage>
	{
		public virtual bool  CanDebug      => Debugger.IsAttached;
		public virtual bool  UseDebugLogin => CanDebug;
		public         Guid? InstallId     { get; protected set; }

		protected bool _ApiEnabled { get; private set; }

		private BaseFileSystemApi? _fileSystemApi;
		private IAppSettings<TDeviceID, TViewPage>? _services;

		protected IAppSettings<TDeviceID, TViewPage> _Services
		{
			get => _services ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
			private set => _services = value;
		}


	#region Init

		public void Init( BaseFileSystemApi api, IAppSettings<TDeviceID, TViewPage> services, string app_center_id, params Type[] appCenterServices )
		{
			Task.Run(async () => await InitAsync(api, services, app_center_id, appCenterServices).ConfigureAwait(true));
		}

		public async Task InitAsync( BaseFileSystemApi api, IAppSettings<TDeviceID, TViewPage> services, string app_center_id, params Type[] appCenterServices )
		{
			_fileSystemApi = api;
			_Services      = services;
			await StartAppCenterAsync(app_center_id, appCenterServices).ConfigureAwait(true);
		}

		public virtual async Task StartAppCenterAsync( string app_center_id, params Type[] services )
		{
			_ApiEnabled = true;

			VersionTracking.Track();

			AppCenter.Start($"ios={app_center_id};android={app_center_id}", services);

			AppCenter.LogLevel = CanDebug
									 ? LogLevel.Verbose
									 : LogLevel.Error; //AppCenter.LogLevel = LogLevel.Debug;

			InstallId =   await AppCenter.GetInstallIdAsync().ConfigureAwait(true);
			InstallId ??= Guid.NewGuid();

			AppCenter.SetUserId(InstallId.ToString());

			_Services.CrashDataPending = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(true);
		}


		protected void ThrowIfNotEnabled()
		{
			if ( _ApiEnabled ) { return; }

			if ( _services is null ) { throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services))); }

			if ( _fileSystemApi is null ) { throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_fileSystemApi))); }

			throw new ApiDisabledException($"Must call {nameof(Init)} first.");
		}

	#endregion


		public void HandleException( Exception e ) { Task.Run(async () => { await HandleExceptionAsync(e).ConfigureAwait(true); }); }

		public async Task HandleExceptionAsync( Exception e )
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			byte[] screenShot = await AppShare.TakeScreenShot().ConfigureAwait(true);

			e.PrintException();

			await TrackError(e, screenShot).ConfigureAwait(true);
		}


	#region AppStates

		protected static async Task SaveAppState( string path, Dictionary<string, object?> payload )
		{
			await using var file = new LocalFile(path);
			await file.WriteToFileAsync(payload).ConfigureAwait(true);
		}

		public async Task SaveFeedBackAppState( Dictionary<string, string?> feedback, string key = "feedback" )
		{
			ThrowIfNotEnabled();

			var result = new Dictionary<string, object?> { [nameof(AppState)] = AppState(), [key] = feedback };

			await SaveAppState(_fileSystemApi!.FeedBackFileName, result).ConfigureAwait(true);
		}


		protected virtual Dictionary<string, string> AppState() =>
			new()
			{
				[nameof(IAppSettings<TDeviceID, TViewPage>.CurrentViewPage)] = _Services.CurrentViewPage?.ToString() ?? throw new NullReferenceException(nameof(_Services.CurrentViewPage)),
				[nameof(IAppSettings<TDeviceID, TViewPage>.AppName)]         = _Services.AppName ?? throw new NullReferenceException(nameof(_Services.AppName)),
				[nameof(DateTime)]                     = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt", CultureInfo.CurrentCulture),
				[nameof(AppDeviceInfo.DeviceId)]       = AppDeviceInfo.DeviceId,
				[nameof(AppDeviceInfo.VersionNumber)]  = AppDeviceInfo.VersionNumber,
				[nameof(LanguageApi.SelectedLanguage)] = CultureInfo.CurrentCulture.DisplayName
			};

	#endregion


	#region Track Exceptions

		public async Task TrackError( Exception e ) =>
			await TrackError(e, e.Details(), e.FullDetails()).ConfigureAwait(true);

		public async Task TrackError( Exception e, byte[] screenShot ) =>
			await TrackError(e, e.Details(), e.FullDetails(), screenShot).ConfigureAwait(true);

		public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails ) =>
			await TrackError(ex, eventDetails, appState: null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, Dictionary<string, object?>? appState ) => await TrackError(ex,
																																						   eventDetails,
																																						   appState,
																																						   null,
																																						   null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, Dictionary<string, object?>? appState, byte[] screenShot ) =>
			await TrackError(ex,
							 eventDetails,
							 appState,
							 null,
							 null,
							 screenShot).ConfigureAwait(true);

		public async Task TrackError( Exception                    ex,
									  Dictionary<string, string?>?  eventDetails,
									  Dictionary<string, object?>? appState,
									  string?                      incomingText,
									  string?                      outgoingText
		) => await TrackError(ex,
							  eventDetails,
							  appState,
							  incomingText,
							  outgoingText,
							  null).ConfigureAwait(true);

		public async Task TrackError( Exception                    ex,
									  Dictionary<string, string?>?  eventDetails,
									  Dictionary<string, object?>? appState,
									  string?                      incomingText,
									  string?                      outgoingText,
									  byte[]?                      screenShot
		)
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			if ( appState is not null ) await SaveAppState(_fileSystemApi!.AppStateFileName, appState).ConfigureAwait(true);

			ErrorAttachmentLog? state = appState is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(appState.ToPrettyJson(), _fileSystemApi!.AppStateFileName);

			ErrorAttachmentLog? debug = eventDetails is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(eventDetails.ToPrettyJson(), _fileSystemApi!.DebugFileName);

			ErrorAttachmentLog? screenShotAttachment = screenShot is null
														   ? null
														   : ErrorAttachmentLog.AttachmentWithBinary(screenShot, "screenShot.jpeg", "image/jpeg");

			var attachments = new List<ErrorAttachmentLog>();

			if ( state is not null ) { attachments.Add(state); }

			if ( debug is not null ) { attachments.Add(debug); }

			if ( screenShotAttachment is not null ) { attachments.Add(screenShotAttachment); }

			if ( !string.IsNullOrWhiteSpace(incomingText) )
			{
				ErrorAttachmentLog incoming = ErrorAttachmentLog.AttachmentWithText(incomingText.ToPrettyJson(), _fileSystemApi!.IncomingFileName);
				attachments.Add(incoming);
			}

			if ( !string.IsNullOrWhiteSpace(outgoingText) )
			{
				ErrorAttachmentLog outgoing = ErrorAttachmentLog.AttachmentWithText(outgoingText.ToPrettyJson(), _fileSystemApi!.OutgoingFileName);
				attachments.Add(outgoing);
			}

			await TrackError(ex, eventDetails, attachments.ToArray()).ConfigureAwait(true);
		}

		public async Task TrackError( Exception ex, Dictionary<string, string?>? eventDetails, params ErrorAttachmentLog[] attachments )
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			if ( ex == null ) throw new ArgumentNullException(nameof(ex));

			Crashes.TrackError(ex, eventDetails, attachments);
			await Task.CompletedTask;
		}

	#endregion


	#region Track Events

		public void TrackEvent( [CallerMemberName] string source = "" )
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			TrackEvent(AppState(), source);
		}

		protected void TrackEvent( Dictionary<string, string> eventDetails, [CallerMemberName] string source = "" )
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			Analytics.TrackEvent(source, eventDetails);
		}

	#endregion
	}
}
