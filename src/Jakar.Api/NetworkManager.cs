using System;
using System.Linq;
using System.Net.Sockets;
using Jakar.Api.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class NetworkManager
	{
		private static readonly INetworkManager _manager = DependencyService.Get<INetworkManager>();

		public static string? GetIdentifier() => _manager.GetIdentifier();
		public static void OpenWifiSettings() => _manager.OpenWifiSettings();
		public static string? GetIPAddress() => _manager.GetIPAddress();

		public static string? GetIPAddressRange()
		{
			string? ip = GetIPAddress();

			return string.IsNullOrWhiteSpace(ip)
					   ? null
					   : ip.Substring(0, ip.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1);
		}

		public static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
		public static bool IsWiFiConnected => IsConnected && Connectivity.ConnectionProfiles.Any(p => p == ConnectionProfile.WiFi || p == ConnectionProfile.Ethernet);



		public class WifiConfig
		{
			public bool JoinOnce { get; }
			public string Ssid { get; }
			public string Password { get; }

			public WifiConfig( string ssid, string password, bool joinOnce )
			{
				Ssid = ssid;
				Password = password;
				JoinOnce = joinOnce;
			}

			public WifiConfig( string ssid, string password ) : this(ssid, password, true) { }
		}



		public static void ThrowIfNotConnected()
		{
			if ( IsConnected ) return;

			throw new SocketException((int) SocketError.NetworkDown);
		}
	}
}
