using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Foundation;
using Jakar.Api.Interfaces;
using Jakar.Api.iOS.Services;
using Jakar.Extensions.Exceptions.General;
using UIKit;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
[assembly: Dependency(typeof(IpAddressManager))]

#nullable enable
namespace Jakar.Api.iOS.Services
{
	public class IpAddressManager : INetworkManager
	{
		public string? GetIpAddress()
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

		public string GetIdentifier() => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

		public void OpenWifiSettings()
		{
			try
			{
				using var wifiUrl = new NSUrl(@"prefs:root=WIFI");
				if ( UIApplication.SharedApplication.CanOpenUrl(wifiUrl) )
				{
					// Pre iOS 10
					UIApplication.SharedApplication.OpenUrl(wifiUrl);
				}
				else
				{
					// iOS 10
					using var nSUrl = new NSUrl(@"App-Prefs:root=WIFI");
					UIApplication.SharedApplication.OpenUrl(nSUrl);
				}
			}
			catch ( Exception ex )
			{
				throw new WiFiException("Could not open Wifi Settings", ex);
			}
		}
    }
}