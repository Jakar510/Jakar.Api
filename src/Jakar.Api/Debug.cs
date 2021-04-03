using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jakar.Api.Extensions;
using Jakar.Api.Models;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class Debug
	{
		public static Debug Current => _Service.Value;
		private static Lazy<Debug> _Service { get; } = new(Create, false);
		private static Debug Create() => new();

		public bool CanDebug => Debugger.IsAttached;
		public bool UseDebugLogin => CanDebug;
		public Guid? InstallId { get; protected set; }


		public virtual async Task StartAppCenter( string app_center_id )
		{
			VersionTracking.Track();
			AppCenter.Start($"ios={app_center_id};android={app_center_id}", typeof(Analytics), typeof(Crashes));

			AppCenter.LogLevel = CanDebug
									 ? LogLevel.Verbose
									 : LogLevel.Error; //AppCenter.LogLevel = LogLevel.Debug;

			InstallId = await AppCenter.GetInstallIdAsync().ConfigureAwait(true);
			AppCenter.SetUserId(InstallId?.ToString());

			AppSettings.CrashDataPending = await Crashes.HasCrashedInLastSessionAsync().ConfigureAwait(true);
		}


		public void HandleException( Exception e ) { Task.Run(async () => { await HandleExceptionAsync(e).ConfigureAwait(true); }); }

		public async Task HandleExceptionAsync( Exception e )
		{
			if ( !AppSettings.Current.SendCrashes ) { return; }

			byte[] screenShot = await Share.TakeScreenShot().ConfigureAwait(true);

			//Dictionary<string, string> dict = GetAppStateFromError(e); // HandleException
			//Dictionary<string, object> state = GetAppState(e);
			PrintException(e); // HandleException

			await TrackError(e, screenShot).ConfigureAwait(true);
		}

		protected async Task SaveAppState( IDictionary<string, object?> payload )
		{
			await using var file = new FileData(FileSystem.AppStateFileName);

			try { await file.WriteToFileAsync(payload).ConfigureAwait(true); }
			catch ( Exception ex )
			{
				PrintException(ex); // SaveAppState
			}
		}

		public async Task<bool> SaveFeedBackAppState( IDictionary<string, string> feedback )
		{
			try
			{
				IDictionary<string, object?> result = GetAppState();
				result["feedback"] = feedback;

				await using var file = new FileData(FileSystem.AccountsFileName);
				await file.WriteToFileAsync(result).ConfigureAwait(true);

				return true;
			}
			catch ( Exception ex )
			{
				PrintMessage(ex.ToString()); // SaveFeedBackAppState
				Crashes.TrackError(ex);
				return false;
			}
		}


		protected void PrintException( Exception e )
		{
			if ( !CanDebug ) { return; }

			System.Diagnostics.Debug.WriteLine("------------------------------------------------ Exception Start -----------------------------------------------\n\n");
			System.Diagnostics.Debug.WriteLine(e.Source);
			System.Diagnostics.Debug.WriteLine("\n----------------------------------------------------------------------------------------------------------------\n");
			System.Diagnostics.Debug.WriteLine(e.Data);
			System.Diagnostics.Debug.WriteLine("\n----------------------------------------------------------------------------------------------------------------\n");
			System.Diagnostics.Debug.WriteLine(e.ToString());
			System.Diagnostics.Debug.WriteLine("\n----------------------------------------------------------------------------------------------------------------\n");
			System.Diagnostics.Debug.WriteLine(e.StackTrace);
			System.Diagnostics.Debug.WriteLine("\n\n------------------------------------------------ Exception End -------------------------------------------------");
		}

		protected virtual IDictionary<string, string> GetAppStateFromError( Exception e )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));

			Dictionary<string, string> dict = new()
											  {
												  ["activeTreeLevel"] = AppSettings.Current.CurrentViewPage?.ToString() ?? throw new NullReferenceException(nameof(AppSettings.CurrentViewPage)),
												  ["AppName"] = AppSettings.Current.AppName ?? throw new NullReferenceException(nameof(AppSettings.AppName)),
												  ["Date"] = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt", Language.Current.CultureInfo),
												  ["Device"] = DeviceInfo.DeviceId,
												  ["CurrentVersion"] = DeviceInfo.VersionNumber,
												  ["source"] = e.Source,
												  ["CurrentLanguage"] = Language.Current.SelectedLanguage.DisplayName
											  };

			IDictionary t = e.Data;
			foreach ( DictionaryEntry o in t.Cast<DictionaryEntry>().Where(o => o.Key is { } && o.Value is { }) ) { dict[o.Key.ToString()] = o.Value.ToString(); }

			return dict;
		}

		protected virtual IDictionary<string, object?>? GetInnerExceptions( Exception e, IDictionary<string, object?> dict )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));
			if ( e.InnerException is null ) { return null; }

			Dictionary<string, object?> inner = new()
												{
													[nameof(e.Message)] = e.InnerException.Message,
													[nameof(e.Source)] = e.InnerException.Source,
													[nameof(e.StackTrace)] = e.InnerException.StackTrace,
													[nameof(e.ToString)] = e.InnerException.ToString(),
												};

			IDictionary<string, object?>? result = GetInnerExceptions(e, inner);
			if ( result is null ) { return dict; }

			dict[nameof(e.InnerException)] = result;

			return dict;
		}

		protected virtual IDictionary<string, object?> GetAppState( Exception e )
		{
			if ( e is null ) throw new ArgumentNullException(nameof(e));
			IDictionary<string, object?> dict = GetAppState();

			dict[nameof(e.Message)] = e.Message;
			dict[nameof(e.Source)] = e.Source;
			dict[nameof(e.StackTrace)] = e.StackTrace;
			dict[nameof(e.ToString)] = e.ToString();

			var inner = new Dictionary<string, object?>();
			dict[nameof(e.InnerException)] = GetInnerExceptions(e, inner) ?? inner;

			IDictionary t = e.Data;
			foreach ( DictionaryEntry o in t ) { dict[o.Key.ToString()] = o.Value.ToString(); }

			return dict;
		}

		protected virtual IDictionary<string, object?> GetAppState() =>
			new Dictionary<string, object?>()
			{
				["activeTreeLevel"] = AppSettings.Current.CurrentViewPage?.ToString(),
				["AppName"] = AppSettings.Current.AppName,
				["Date"] = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt", Language.Current.CultureInfo),
				["Device"] = DeviceInfo.DeviceId,
				["CurrentVersion"] = DeviceInfo.VersionNumber,
				["CurrentLanguage"] = Language.Current.SelectedLanguage.DisplayName,
			};


		public async Task TrackError( Exception e ) => await TrackError(e, GetAppStateFromError(e), GetAppState(e)).ConfigureAwait(true);
		public async Task TrackError( Exception e, byte[] screenShot ) => await TrackError(e, GetAppStateFromError(e), GetAppState(e), screenShot).ConfigureAwait(true);
		public async Task TrackError( Exception ex, IDictionary<string, string>? eventDetails ) => await TrackError(ex, eventDetails, appState: null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, IDictionary<string, string>? eventDetails, IDictionary<string, object?>? appState ) => await TrackError(ex,
																																							eventDetails,
																																							appState,
																																							null,
																																							null).ConfigureAwait(true);

		public async Task TrackError( Exception ex, IDictionary<string, string>? eventDetails, IDictionary<string, object?>? appState, byte[] screenShot ) =>
			await TrackError(ex,
							 eventDetails,
							 appState,
							 null,
							 null,
							 screenShot).ConfigureAwait(true);

		public async Task TrackError( Exception ex,
									  IDictionary<string, string>? eventDetails,
									  IDictionary<string, object?>? appState,
									  string? incomingText,
									  string? outgoingText
		) => await TrackError(ex,
							  eventDetails,
							  appState,
							  incomingText,
							  outgoingText,
							  null).ConfigureAwait(true);

		public async Task TrackError( Exception ex,
									  IDictionary<string, string>? eventDetails,
									  IDictionary<string, object?>? appState,
									  string? incomingText,
									  string? outgoingText,
									  byte[]? screenShot
		)
		{
			if ( !AppSettings.Current.SendCrashes ) { return; }

			if ( appState is not null ) await SaveAppState(appState).ConfigureAwait(true);

			ErrorAttachmentLog? state = appState is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(appState.ToPrettyJson(), FileSystem.AppStateFileName);

			ErrorAttachmentLog? debug = eventDetails is null
											? null
											: ErrorAttachmentLog.AttachmentWithText(eventDetails.ToPrettyJson(), FileSystem.DebugFileName);

			ErrorAttachmentLog? screenShotAttachment = screenShot is null
														   ? null
														   : ErrorAttachmentLog.AttachmentWithBinary(screenShot, "screenShot.jpeg", "image/jpeg");

			var attachments = new List<ErrorAttachmentLog>();

			if ( state is not null ) { attachments.Add(state); }

			if ( debug is not null ) { attachments.Add(debug); }

			if ( screenShotAttachment is not null ) { attachments.Add(screenShotAttachment); }

			if ( !string.IsNullOrWhiteSpace(incomingText) )
			{
				ErrorAttachmentLog incoming = ErrorAttachmentLog.AttachmentWithText(incomingText.ToPrettyJson(), FileSystem.IncomingFileName);
				attachments.Add(incoming);
			}

			if ( !string.IsNullOrWhiteSpace(outgoingText) )
			{
				ErrorAttachmentLog outgoing = ErrorAttachmentLog.AttachmentWithText(outgoingText.ToPrettyJson(), FileSystem.OutgoingFileName);
				attachments.Add(outgoing);
			}

			await TrackError(ex, eventDetails, attachments.ToArray()).ConfigureAwait(true);
		}

		public Task TrackError( Exception ex, IDictionary<string, string>? eventDetails, params ErrorAttachmentLog[] attachments )
		{
			if ( !AppSettings.Current.SendCrashes ) { return Task.CompletedTask; }

			if ( ex == null ) throw new ArgumentNullException(nameof(ex));

			Crashes.TrackError(ex, eventDetails, attachments);
			return Task.CompletedTask;
		}


		public void TrackEvent( string source )
		{
			if ( !AppSettings.Current.SendCrashes ) { return; }

			TrackEvent(source,
					   new Dictionary<string, string>()
					   {
						   ["DeviceID"] = DeviceInfo.DeviceId,
						   ["AppName"] = AppSettings.Current.AppName ?? throw new NullReferenceException(nameof(AppSettings.Current.AppName)),
						   ["Date"] = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt", Language.Current.CultureInfo),
						   ["CurrentLanguage"] = Language.Current.SelectedLanguage.DisplayName,
					   });
		}

		protected void TrackEvent( string source, IDictionary<string, string> eventDetails )
		{
			if ( !AppSettings.Current.SendCrashes ) { return; }

			Analytics.TrackEvent(source, eventDetails);
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
	}
}
