using Android.OS;
using Android.Runtime;

#pragma warning disable 1591

namespace Jakar.Api.Droid
{
	public abstract class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
#pragma warning disable 8618
		public static MainActivity Instance { get; protected set; }
#pragma warning restore 8618
		
		/// <summary>
		/// Add to the subclass: "base.OnCreate(savedInstanceState); LoadApplication(new App());"
		/// </summary>
		/// <param name="savedInstanceState"></param>
		protected override void OnCreate( Bundle savedInstanceState )
		{
			base.OnCreate(savedInstanceState);

			Xamarin.Forms.Forms.SetFlags("MediaElement_Experimental");
			Xamarin.Forms.Forms.Init(this, savedInstanceState);
			Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			
			Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
			Acr.UserDialogs.UserDialogs.Init(this);
			
			Instance = this;
		}
		public override void OnRequestPermissionsResult( int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults )
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}