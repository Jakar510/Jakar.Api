using System;
using System.Linq;
using System.Net;
using Android.Content;
using Android.Provider;
using Jakar.Api.Droid.Services;
using Jakar.Api.Exceptions.General;
using Jakar.Api.Interfaces;
using Xamarin.Forms;


#pragma warning disable 1591

[assembly: Dependency(typeof(IPAddressManager))]

#nullable enable
namespace Jakar.Api.Droid.Services
{
	[global::Android.Runtime.Preserve(AllMembers = true)]
	public class IPAddressManager : INetworkManager
	{
		public void OpenWifiSettings()
		{
			try
			{
				using var intent = new Intent(Settings.ActionWifiSettings);
				BaseApplication.Instance.StartActivity(intent);
			}
			catch ( Exception ex ) { throw new WiFiException("Opening Wifi settings was not possible", ex); }
		}

		public string? GetIdentifier() => Settings.Secure.GetString(global::Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId);
		public string? GetIPAddress() => Dns.GetHostAddresses(Dns.GetHostName()).Select(address => address.ToString()).FirstOrDefault();
	}
}
