using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class UserPreferences
	{
		public static string GetPreferenceName( object value, string name ) => GetName(value.GetType(), name);
		public static string GetPreferenceName( Type type, string name ) => GetName(type, name);
		public static string GetName( Type type, string name, [CallerMemberName] string caller = "" ) => $"{ApiServices.Current.AppName}.{type.FullName}.{caller}.{name}";
		public static string GetShortName( Type type, string name ) => $"{ApiServices.Current.AppName}.{type.FullName}.{name}";


		//public static string GetPreferenceName(object type, string name, [CallerMemberName] string caller = "") => GetName(type?.GetType(), name, caller);
		//public static string GetPreferenceName(string name, [CallerMemberName] string caller = "") => GetName(name, caller);
		//public static string GetName(string name, string caller) => $"{App.AppName}.{caller}.{name}";


		public static string GetPassword( string key ) => GetPasswordAsync(key).Result;

		public static async Task<string> GetPasswordAsync( string key )
		{
			try { return await SecureStorage.GetAsync(key).ConfigureAwait(true); }
			catch
			{
				// Possible that device doesn't support secure storage on device.		
				return string.Empty;
			}
		}

		public static void SetPassword( string key, string value ) => MainThread.InvokeOnMainThreadAsync(async () => await SetPasswordAsync(key, value).ConfigureAwait(true)).Wait();

		public static async Task SetPasswordAsync( string key, string value ) { await SecureStorage.SetAsync(key, value).ConfigureAwait(true); }
	}
}
