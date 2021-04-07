namespace Jakar.Api.Interfaces
{
	// Ip Address
	// https://theconfuzedsourcecode.wordpress.com/2015/05/16/how-to-easily-get-device-ip-address-in-xamarin-forms-using-dependencyservice/

	// Open WiFi Settings
	// https://gist.github.com/zuckerthoben/ee097b816b88491f5874bb327ec6819c

	// Xamarin Android Get Device Mac Address
	// https://gist.github.com/tomcurran/099c8e74bc094f5d285867dcea0b63d4

	// Mac Address
	// https://www.tutorialspoint.com/how-to-get-the-mac-address-of-an-ios-iphone-programmatically
	// https://stackoverflow.com/questions/50232847/how-to-get-device-id-in-xamarin-forms

	// connect to WiFi
	// https://spin.atomicobject.com/2018/02/15/connecting-wifi-xamarin-forms/
	// https://c-sharx.net/programmatically-connecting-your-xamarin-ios-app-to-a-wifi-network-using-nehotspotconfigurationmanager
	// https://github.com/Krumelur/WifiDemo
	// https://stackoverflow.com/questions/8818290/how-do-i-connect-to-a-specific-wi-fi-network-in-android-programmatically
	// https://gist.github.com/kraigspear/2c3de568cc7ae3c5c360bcac7e9db92a


	#pragma warning disable 1591

	#nullable enable
	public interface INetworkManager
	{
		public string? GetIdentifier();
		public string? GetIpAddress();

		public void OpenWifiSettings();

		//public void ConnectToWifi(string ssid, string password) => ConnectToWifi(new WifiConfig(ssid, password));
		//public void ConnectToWifi(WifiConfig configuration);
	}
}
