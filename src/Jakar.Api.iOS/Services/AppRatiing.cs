using System;
using Foundation;
using Jakar.Api.Interfaces;
using Jakar.Api.iOS.Services;
using Jakar.Api.Statics;
using StoreKit;
using UIKit;


#pragma warning disable 1591

#nullable enable
[assembly: Xamarin.Forms.Dependency(typeof(AppRating))]

namespace Jakar.Api.iOS.Services
{
	public class AppRating : IAppRating
	{
		public void RateApp()
		{
			if ( UIDevice.CurrentDevice.CheckSystemVersion(10, 3) ) { SKStoreReviewController.RequestReview(); }
			else
			{
				string storeUrl = $@"itms-apps://itunes.apple.com/app/{AppDeviceInfo.PackageName}?action=write-review";

				try
				{
					using var uri = new NSUrl(storeUrl);
					UIApplication.SharedApplication.OpenUrl(uri);
				}
				catch ( Exception ex )
				{
					// Here you could show an alert to the user telling that App Store was unable to launch

					Console.WriteLine(ex.Message);
				}
			}
		}
	}
}
