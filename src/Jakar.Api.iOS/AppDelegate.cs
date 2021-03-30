using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs.Infrastructure;
using Foundation;
using UIKit;

namespace Jakar.Api.iOS
{
	/// <summary>
	/// The UIApplicationDelegate for the application.
	/// This class is responsible for launching the User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	/// </summary>
	[Register(nameof(AppDelegate))]
	public abstract class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		/// <summary>
		/// This method is invoked when the application has loaded and is ready to run.
		/// In this method you should instantiate the window, load the UI into it and then make the window visible.
		/// You have 17 seconds to return from this method, or iOS will terminate your application.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="options"></param>
		public override bool FinishedLaunching( UIApplication app, NSDictionary options )
		{
			Init("MediaElement_Experimental");
			return base.FinishedLaunching(app, options);
		}

		/// <summary>
		/// Add to the subclass: "LoadApplication(new App());"
		/// </summary>
		protected void Init( params string[] flags )
		{
			Xamarin.Forms.Forms.SetFlags(flags);
			Xamarin.Forms.Forms.Init();
			Xamarin.Forms.FormsMaterial.Init();

			Acr.UserDialogs.UserDialogs.Init(() => UIApplication.SharedApplication.GetTopViewController());

			// Add LoadApplication(new App()); here
		}
	}
}