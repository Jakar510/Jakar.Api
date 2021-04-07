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
		public static string? GetIpAddress() => _manager.GetIpAddress();

		public static string? GetIpAddressRange()
		{
			string? ip = GetIpAddress();

			return string.IsNullOrWhiteSpace(ip)
					   ? null
					   : ip.Substring(0, ip.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1);
		}

		public static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
		public static bool IsWiFiConnected => IsConnected && Connectivity.ConnectionProfiles.Any(p => p == ConnectionProfile.WiFi || p == ConnectionProfile.Ethernet);


		public static void ThrowIfNotConnected()
		{
			if ( IsConnected ) return;

			throw new SocketException((int) SocketError.NetworkDown);
		}
	}
}
