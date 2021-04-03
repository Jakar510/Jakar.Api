using System;
using Jakar.Api.Droid.Services;
using Jakar.Api.Interfaces;


#pragma warning disable 1591

[assembly: Xamarin.Forms.Dependency(typeof(BaseUrlAndroid))]

namespace Jakar.Api.Droid.Services
{
	[global::Android.Runtime.Preserve(AllMembers = true)]
	public class BaseUrlAndroid : IBaseUrl
	{
		public BaseUrlAndroid() { }
		public string GetBaseString() => "file:///android_asset/";

		public Uri GetUri() => new(GetBaseString());
	}
}
