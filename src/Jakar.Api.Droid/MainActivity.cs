using Android.OS;
using Android.Runtime;

#pragma warning disable 1591

namespace Jakar.Api.Droid
{
	public abstract class BaseApplication : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
#pragma warning disable 8618
		public static BaseApplication Instance { get; protected set; }
#pragma warning restore 8618
		


		protected void Init( Bundle savedInstanceState, params string[] flags )
		{
			base.OnCreate(savedInstanceState);

			Xamarin.Forms.Forms.SetFlags(flags);
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