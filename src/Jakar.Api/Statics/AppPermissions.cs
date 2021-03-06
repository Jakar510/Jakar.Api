//using Plugin.Permissions;
//using Plugin.Permissions.Abstractions;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Statics
{
	public static class AppPermissions
	{
		public static async Task<T?> Handle<T>( PermissionStatus status, Func<Task<T>>? denial, Func<Task<T>>? granted, Func<Task<T>>? unknown )
		{
			return status switch
				   {
					   //Query permission
					   PermissionStatus.Granted when granted is null => default,
					   PermissionStatus.Granted                      => await granted().ConfigureAwait(true),

					   //Permission denied
					   PermissionStatus.Denied when denial is null => default,
					   PermissionStatus.Denied                     => await denial().ConfigureAwait(true),

					   //Permission denied
					   PermissionStatus.Unknown when unknown is null => default,
					   PermissionStatus.Unknown                      => await unknown().ConfigureAwait(true),

					   _ => default
				   };
		}

		public static async Task Handle( PermissionStatus status, Func<Task>? denial, Func<Task>? granted, Func<Task>? unknown )
		{
			switch ( status )
			{
				//Query permission
				case PermissionStatus.Granted when granted is null:
					return;

				case PermissionStatus.Granted:
					await granted().ConfigureAwait(true);
					break;

				//Permission denied
				case PermissionStatus.Unknown when unknown is null:
					return;

				case PermissionStatus.Unknown:
					await unknown().ConfigureAwait(true);
					break;

				case PermissionStatus.Denied: // Notify user permission was denied
					if ( denial is null )
						return;

					await denial().ConfigureAwait(true);
					break;

				case PermissionStatus.Disabled:
					break;

				case PermissionStatus.Restricted:
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}

		public static T? Handle<T>( PermissionStatus status, Func<T>? denial, Func<T>? granted, Func<T>? unknown )
		{
			return status switch
				   {
					   //Query permission
					   PermissionStatus.Granted when granted is null => default,
					   PermissionStatus.Granted                      => granted(),

					   //Permission denied
					   PermissionStatus.Unknown when unknown is null => default,
					   PermissionStatus.Unknown                      => unknown(),

					   // Notify user permission was denied
					   PermissionStatus.Denied when denial is null => default,
					   PermissionStatus.Denied                     => denial(),

					   PermissionStatus.Disabled   => default,
					   PermissionStatus.Restricted => default,
					   _                           => throw new ArgumentOutOfRangeException(nameof(status), status, null)
				   };
		}

		public static void Handle( PermissionStatus status, Action? denial, Action? granted, Action? unknown )
		{
			switch ( status )
			{
				//Query permission
				case PermissionStatus.Granted when granted is null:
					return;

				case PermissionStatus.Granted:
					granted();
					return;

				//Permission denied
				case PermissionStatus.Unknown when unknown is null:
					return;

				case PermissionStatus.Unknown:
					unknown();
					return;

				case PermissionStatus.Denied when denial is null:
					return;

				case PermissionStatus.Denied:
					denial();
					return;

				case PermissionStatus.Disabled:
					break;

				case PermissionStatus.Restricted:
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}

		public static void Handle( PermissionStatus status, ICommand? denial, ICommand? granted, ICommand? unknown )
		{
			switch ( status )
			{
				//Query permission
				case PermissionStatus.Granted when granted is null:
					return;

				case PermissionStatus.Granted:
					granted.Execute(null);
					return;

				//Permission denied
				case PermissionStatus.Unknown when unknown is null:
					return;

				case PermissionStatus.Unknown:
					unknown.Execute(null);
					return;

				case PermissionStatus.Denied when denial is null:
					return;

				case PermissionStatus.Denied:
					denial.Execute(null);
					return;

				case PermissionStatus.Disabled:
					break;

				case PermissionStatus.Restricted:
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}

		public static void Handle<T>( PermissionStatus status,
									  Command<T>?      denial,
									  Command<T>?      granted,
									  Command<T>?      unknown,
									  T                obj
		)
		{
			switch ( status )
			{
				//Query permission
				case PermissionStatus.Granted when granted is null:
					return;

				case PermissionStatus.Granted:
					if ( granted.CanExecute(obj) )
						granted.Execute(obj);

					return;

				//Permission denied
				case PermissionStatus.Unknown when unknown is null:
					return;

				case PermissionStatus.Unknown:
					if ( unknown.CanExecute(obj) )
						unknown.Execute(obj);

					return;

				case PermissionStatus.Denied when denial is null:
					return;

				case PermissionStatus.Denied:
					if ( denial.CanExecute(obj) )
						denial.Execute(obj);

					return;

				case PermissionStatus.Disabled:
					break;

				case PermissionStatus.Restricted:
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}


		private static async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>( T permission ) where T : Xamarin.Essentials.Permissions.BasePermission, new()
		{
			PermissionStatus status = await permission.CheckStatusAsync().ConfigureAwait(true);

			if ( status != PermissionStatus.Granted ) { status = await permission.RequestAsync().ConfigureAwait(true); }

			return status;
		}

		public static async Task<PermissionStatus> CameraPermission()            => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Camera()).ConfigureAwait(true);
		public static async Task<PermissionStatus> StorageReadPermission()       => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.StorageRead()).ConfigureAwait(true);
		public static async Task<PermissionStatus> StorageWritePermission()      => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.StorageWrite()).ConfigureAwait(true);
		public static async Task<PermissionStatus> MediaLibraryPermission()      => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Media()).ConfigureAwait(true);
		public static async Task<PermissionStatus> SensorsPermission()           => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Sensors()).ConfigureAwait(true);
		public static async Task<PermissionStatus> LocationWhenInUsePermission() => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.LocationWhenInUse()).ConfigureAwait(true);
		public static async Task<PermissionStatus> LocationAlwaysPermission()    => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.LocationAlways()).ConfigureAwait(true);
		public static async Task<PermissionStatus> PhotosPermission()            => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Photos()).ConfigureAwait(true);
		public static async Task<PermissionStatus> CalendarReadPermission()      => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.CalendarRead()).ConfigureAwait(true);
		public static async Task<PermissionStatus> CalendarWritePermission()     => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.CalendarWrite()).ConfigureAwait(true);
		public static async Task<PermissionStatus> RemindersPermission()         => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Reminders()).ConfigureAwait(true);
		public static async Task<PermissionStatus> SmsPermission()               => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Sms()).ConfigureAwait(true);
		public static async Task<PermissionStatus> ContactsReadPermission()      => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.ContactsRead()).ConfigureAwait(true);
		public static async Task<PermissionStatus> ContactsWritePermission()     => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.ContactsWrite()).ConfigureAwait(true);
		public static async Task<PermissionStatus> MicrophonePermission()        => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Microphone()).ConfigureAwait(true);
		public static async Task<PermissionStatus> PhonePermission()             => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Phone()).ConfigureAwait(true);
		public static async Task<PermissionStatus> SpeechPermission()            => await CheckAndRequestPermissionAsync(new Xamarin.Essentials.Permissions.Speech()).ConfigureAwait(true);

		/*

		public enum Permission
		{
			Unknown = 0,
			Calendar = 1,
			Camera = 2,
			Contacts = 3,
			Location = 4,
			Microphone = 5,
			Phone = 6,
			Photos = 7,
			Reminders = 8,
			Sensors = 9,
			Sms = 10,
			Storage = 11,
			Speech = 12,
			LocationAlways = 13,
			LocationWhenInUse = 14,
			MediaLibrary = 15
		}

		*/
	}
}
