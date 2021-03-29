using System;
using Foundation;
using StoreKit;
using TrueLogicMobile.API;
using TrueLogicMobile.API.Interfaces;
using TrueLogicMobile.iOS.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(AppRating))]
namespace TrueLogicMobile.iOS.Services
{
	public class AppRating : IAppRating
	{
		public void RateApp()
		{
			if ( UIDevice.CurrentDevice.CheckSystemVersion(10, 3) )
				SKStoreReviewController.RequestReview();
			else
			{
				string storeUrl = $@"itms-apps://itunes.apple.com/app/{Api.DeviceInfo.PackageName}?action=write-review";

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