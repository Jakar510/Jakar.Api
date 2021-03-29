using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Foundation;
using TrueLogicMobile.API;
using TrueLogicMobile.API.Exceptions.General;
using TrueLogicMobile.API.Interfaces;
using TrueLogicMobile.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IPAddressManager))]

#nullable enable
namespace TrueLogicMobile.iOS.Services
{
	public class IPAddressManager : INetworkManager
	{
		public string? GetIPAddress()
		{
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