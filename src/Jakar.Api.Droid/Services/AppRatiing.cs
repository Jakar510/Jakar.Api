﻿using System;
using Android.Content;
using Android.Content.PM;
using Jakar.Api.Droid.Services;
using Jakar.Api.Interfaces;
using AUri = Android.Net.Uri;


#pragma warning disable 1591

[assembly: Xamarin.Forms.Dependency(typeof(AppRating))]

namespace Jakar.Api.Droid.Services
{
	public class AppRating : IAppRating
	{
		public void RateApp()
		{
			Context context = global::Android.App.Application.Context;
			string url = $"market://details?id={DeviceInfo.PackageName}";

			try
			{
				context.PackageManager?.GetPackageInfo("com.android.vending", PackageInfoFlags.Activities);
				using var intent = new Intent(Intent.ActionView, AUri.Parse(url));

				context.StartActivity(intent);
			}
			catch ( PackageManager.NameNotFoundException ex )
			{
				// this won't happen. But catching just in case the user has downloaded the app without having Google Play installed.

				Console.WriteLine(ex.Message);
			}
			catch ( ActivityNotFoundException )
			{
				// if Google Play fails to load, open the App link on the browser 

				string playStoreUrl = $@"https://play.google.com/store/apps/details?id={DeviceInfo.PackageName}"; //Add here the url of your application on the store

				var browserIntent = new Intent(Intent.ActionView, AUri.Parse(playStoreUrl));
				browserIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);

				context.StartActivity(browserIntent);
			}
		}
	}
}
