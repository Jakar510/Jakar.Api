using System;
using Foundation;
using Jakar.Api.Interfaces;
using Jakar.Api.iOS.Services;


#pragma warning disable 1591

#nullable enable
[assembly: Xamarin.Forms.Dependency(typeof(BaseUrlIos))]
namespace Jakar.Api.iOS.Services
{
	public class BaseUrlIos : IBaseUrl
	{
		public BaseUrlIos() { }

		public string GetBaseString() => NSBundle.MainBundle.BundlePath;

		public Uri GetUri() => new (GetBaseString());
	}
}