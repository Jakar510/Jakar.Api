using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Jakar.Api.Exceptions.General;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Jakar.Api.Models;
using Jakar.Api.Statics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using DeviceInfo = Jakar.Api.Statics.DeviceInfo;
using Share = Jakar.Api.Statics.Share;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class Debug
	{
		public bool CanDebug => Debugger.IsAttached;
		public bool UseDebugLogin => CanDebug;
		public Guid? InstallId { get; protected set; }

		protected bool _ApiEnabled { get; private set; }

		private IAppSettings? _services;

		protected IAppSettings _Services
		{
			get => _services ?? throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services)));
			private set => _services = value;
		}


	#region Init

		public void Init( IAppSettings services, string app_center_id, params Type[] appCenterServices )
		{
			_Services = services;

			Task.Run(async () => await StartAppCenterAsync(app_center_id, appCenterServices).ConfigureAwait(true));
		}

		public async Task InitAsync( IAppSettings services, string app_center_id, params Type[] appCenterServices )
		{
			_Services = services;
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

			InstallId = await AppCenter.GetInstallIdAsync().ConfigureAwait(true);
			InstallId ??= Guid.NewGuid();

			AppCenter.SetUserId(InstallId.ToString());

			_Services.CrashDataPending = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(true);
		}


		protected void ThrowIfNotEnabled()
		{
			if ( _ApiEnabled ) { return; }

			if ( _services is null ) { throw new ApiDisabledException($"Must call {nameof(Init)} first.", new NullReferenceException(nameof(_services))); }

			throw new ApiDisabledException($"Must call {nameof(Init)} first.");
		}

	#endregion


		public void HandleException( Exception e ) { Task.Run(async () => { await HandleExceptionAsync(e).ConfigureAwait(true); }); }

		public async Task HandleExceptionAsync( Exception e )
		{
			ThrowIfNotEnabled();

			if ( !_Services.SendCrashes ) { return; }

			byte[] screenShot = await Share.TakeScreenShot().ConfigureAwait(true);

			PrintException(e);

			await TrackError(e, screenShot).ConfigureAwait(true);
		}


	#region AppStates

		protected static async Task SaveAppState( string path, Dictionary<string, object?> payload )
		{
			await using var file = new FileData(path);
			await file.WriteToFileAsync(payload).ConfigureAwait(true);
		}

		public async Task SaveFeedBackAppState( Dictionary<string, string?> feedback, string key = "feedback" )
		{
			var result = new Dictionary<string, object?> { [nameof(AppState)] = AppState(), [key] = feedback };

			await SaveAppState(FileSystemApi.FeedBackFileName, result).ConfigureAwait(true);
		}


		protected virtual Dictionary<string, string> GetAppStateFromError( Exception e )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			Dictionary<string, string> dict = AppState();
			Update(ref dict, e);

			return dict;
		}

		protected virtual void Update( ref Dictionary<string, string> dict, Exception e )
		{
			dict[nameof(e.Source)] = e.Source;
			dict[nameof(e.Message)] = e.Message;
			dict[nameof(e.Source)] = e.Source;
			dict[nameof(e.StackTrace)] = e.StackTrace;
			dict[nameof(e.ToString)] = e.ToString();
		}

		protected virtual void Update( ref Dictionary<string, object?> dict, Exception e )
		{
			dict[nameof(e.Source)] = e.Source;
			dict[nameof(e.Message)] = e.Message;
			dict[nameof(e.Source)] = e.Source;
			dict[nameof(e.StackTrace)] = e.StackTrace;
			dict[nameof(e.ToString)] = e.ToString();
		}

		protected virtual Dictionary<string, object?> AppState( Exception e )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			var inner = new Dictionary<string, object?>();

			var dict = new Dictionary<string, object?>
					   {
						   [nameof(AppState)] = AppState(),
						   [nameof(e.InnerException)] = GetInnerExceptions(e, ref inner),
						   [nameof(e.Data)] = GetData(e)
					   };

			Update(ref dict, e);

			return dict;
		}

		protected virtual Dictionary<string, object?>? GetInnerExceptions( Exception e, ref Dictionary<string, object?> dict )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));
			if ( e.InnerException is null ) { return null; }

			var inner = new Dictionary<string, object?>();
			Update(ref inner, e);

			Dictionary<string, object?>? result = GetInnerExceptions(e, ref inner);
			if ( result is null ) { return dict; }

			dict[nameof(e.InnerException)] = result;

			return dict;
		}

		protected static Dictionary<string, string> GetData( Exception e )
		{
			var data = new Dictionary<string, string>();

			foreach ( DictionaryEntry o in e.Data.Cast<DictionaryEntry>().Where(o => o.Key is not null && o.Value is not null) ) { data[o.Key.ToString()] = o.Value.ToString(); }

			return data;
		}

		protected virtual Dictionary<string, string> AppState() =>
			new()
			{
				[nameof(IAppSettings.CurrentViewPage)] = _Services.CurrentViewPage?.ToString() ?? throw new NullReferenceException(nameof(_Services.CurrentViewPage)),
				[nameof(IAppSettings.AppName)] = _Services.AppName ?? throw new NullReferenceException(nameof(_Services.AppName)),
				[nameof(DateTime)] = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt", LanguageApi.Current.CultureInfo),
				[nameof(DeviceInfo.DeviceId)] = DeviceInfo.DeviceId,
				[nameof(DeviceInfo.VersionNumber)] = DeviceInfo.VersionNumber,
				[nameof(LanguageApi.SelectedLanguage)] = LanguageApi.Current.SelectedLanguage.DisplayName
			};

	#endregion


	#region Track Exceptions

		public async Task TrackError( Exception e ) => await TrackError(e, GetAppStateFromError(e), AppState(e)).ConfigureAwait(true);
		public async Task TrackError( Exception e, byte[] screenShot ) => await TrackError(e, GetAppStateFromError(e), AppState(e), screenShot).ConfigureAwait(true);
		public async Task TrackError( Exception ex, Dictionary<string, string>? eventDetails ) => await TrackError(ex, eventDetails, appState: null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, Dictionary<string, string>? eventDetails, Dictionary<string, object?>? appState ) => await TrackError(ex,
																																						  eventDetails,
																																						  appState,
																																						  null,
																																						  null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, Dictionary<string, string>? eventDetails, Dictionary<string, object?>? appState, byte[] screenShot ) =>
			await TrackError(ex,
							 eventDetails,
							 appState,
							 null,
							 null,
							 screenShot).ConfigureAwait(true);

		public async Task TrackError( Exception ex,
									  Dictionary<string, string>? eventDetails,
									  Dictionary<string, object?>? appState,
									  string? incomingText,
									  string? outgoingText
		) => await TrackError(ex,
							  eventDetails,
							  appState,
							  incomingText,
							  outgoingText,
							  null).ConfigureAwait(true);

		public async Task TrackError( Exception ex,
									  Dictionary<string, string>? eventDetails,
									  Dictionary<string, object?>? appState,
									  string? incomingText,
									  string? outgoingText,
									  byte[]? screenShot
		)
		{
			if ( !_Services.SendCrashes ) { return; }

			if ( appState is not null ) await SaveAppState(FileSystemApi.AppStateFileName, appState).ConfigureAwait(true);

			ErrorAttachmentLog? state = appState is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(appState.ToPrettyJson(), FileSystemApi.AppStateFileName);

			ErrorAttachmentLog? debug = eventDetails is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(eventDetails.ToPrettyJson(), FileSystemApi.DebugFileName);

			ErrorAttachmentLog? screenShotAttachment = screenShot is null
														   ? null
														   : ErrorAttachmentLog.AttachmentWithBinary(screenShot, "screenShot.jpeg", "image/jpeg");

			var attachments = new List<ErrorAttachmentLog>();

			if ( state is not null ) { attachments.Add(state); }

			if ( debug is not null ) { attachments.Add(debug); }

			if ( screenShotAttachment is not null ) { attachments.Add(screenShotAttachment); }

			if ( !string.IsNullOrWhiteSpace(incomingText) )
			{
				ErrorAttachmentLog incoming = ErrorAttachmentLog.AttachmentWithText(incomingText.ToPrettyJson(), FileSystemApi.IncomingFileName);
				attachments.Add(incoming);
			}

			if ( !string.IsNullOrWhiteSpace(outgoingText) )
			{
				ErrorAttachmentLog outgoing = ErrorAttachmentLog.AttachmentWithText(outgoingText.ToPrettyJson(), FileSystemApi.OutgoingFileName);
				attachments.Add(outgoing);
			}

			await TrackError(ex, eventDetails, attachments.ToArray()).ConfigureAwait(true);
		}

		public async Task TrackError( Exception ex, Dictionary<string, string>? eventDetails, params ErrorAttachmentLog[] attachments )
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


	#region Console

		protected virtual void PrintException( Exception e )
		{
			if ( !CanDebug ) { return; }

			System.Diagnostics.Debug.WriteLine("------------------------------------------------ Exception Start -----------------------------------------------");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine("-------------------------------------------------- Source ------------------------------------------------");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine($"{e.Source}");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine("--------------------------------------------------- Data -------------------------------------------------");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine($"{e.Data.ToPrettyJson()}");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine("------------------------------------------------- ToString -----------------------------------------------");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine($"{e}");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine("------------------------------------------------ StackTrace -----------------------------------------------");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine($"{e.StackTrace}");
			System.Diagnostics.Debug.WriteLine("\n\n");
			System.Diagnostics.Debug.WriteLine("------------------------------------------------ Exception End -------------------------------------------------");
		}

		public void PrintMessage( string s, string start = "--------- information ------------" )
		{
			if ( !CanDebug ) { return; }

			System.Diagnostics.Debug.WriteLine($"{start}  {s}");
		}

		public void PrintError( string s, string start = "--------- warning ------------" )
		{
			if ( !CanDebug ) { return; }

			System.Diagnostics.Debug.WriteLine($"{start}  {s}");
		}


		public void PrintJson( object jsonSerializablePayload ) => PrintJson(jsonSerializablePayload.ToPrettyJson());

		public void PrintJson( string jsonString )
		{
			if ( string.IsNullOrWhiteSpace(jsonString) || !CanDebug ) { return; }

			PrintMessage(jsonString.ToPrettyJson());
		}

		public void PrintCount( string source, int count, string start = "----------------------------------------------------------------------------" )
		{
			if ( !CanDebug ) { return; }

			System.Diagnostics.Debug.WriteLine($"{start}   {source}.Count: => {count}");
		}

	#endregion
	}
}
