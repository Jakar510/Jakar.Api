using System;
using Foundation;
using TrueLogicMobile.API;
using TrueLogicMobile.API.Interfaces;
using TrueLogicMobile.iOS.Services;

[assembly: Xamarin.Forms.Dependency(typeof(BaseUrliOS))]
namespace TrueLogicMobile.iOS.Services
{
	public class BaseUrliOS : IBaseUrl
	{
		public BaseUrliOS() { }

		public string GetBaseString() => NSBundle.MainBundle.BundlePath;

		public Uri GetUri() => new Uri(GetBaseString());
	}
}