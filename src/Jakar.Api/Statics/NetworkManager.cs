using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Jakar.Api.Interfaces;
using Jakar.Extensions.General;
using Xamarin.Essentials;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Statics
{
	public static class NetworkManager
	{
		private static readonly INetworkManager _manager = DependencyService.Resolve<INetworkManager>();

		public static string? GetIdentifier()
		{
			if ( _manager is null ) { throw new NullReferenceException(nameof(_manager)); }

			return _manager.GetIdentifier();
		}

		public static void OpenWifiSettings()
		{
			if ( _manager is null ) { throw new NullReferenceException(nameof(_manager)); }

			_manager.OpenWifiSettings();
		}

		public static string? GetIpAddressRange()
		{
			string? ip = GetIpAddress();

			return string.IsNullOrWhiteSpace(ip)
					   ? null
					   : ip[..( ip.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1 )];
		}

		public static string? GetIpAddress()
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach ( NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces() )
			{
				if ( netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet ) continue;

				foreach ( UnicastIPAddressInformation addressInfo in netInterface.GetIPProperties().UnicastAddresses )
				{
					if ( addressInfo.Address.AddressFamily != AddressFamily.InterNetwork ) continue;

					var ipAddress = addressInfo.Address.ToString();

					if ( ipAddress.Contains("169.254", StringComparison.OrdinalIgnoreCase) ) continue;

					return ipAddress;
				}
			}

			return default;
		}


		public static bool IsConnected     => Connectivity.NetworkAccess == NetworkAccess.Internet;
		public static bool IsWiFiConnected => IsConnected && Connectivity.ConnectionProfiles.Any(p => p == ConnectionProfile.WiFi || p == ConnectionProfile.Ethernet);


		public static void ThrowIfNotConnected()
		{
			if ( IsConnected ) return;

			throw new SocketException(SocketError.NetworkDown.ToInt());
		}
	}
}
