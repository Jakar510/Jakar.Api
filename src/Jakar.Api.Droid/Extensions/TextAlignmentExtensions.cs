using Android.Views;
using Xamarin.Forms;
using ATextAlignment = Android.Views.TextAlignment;
using TextAlignment = Xamarin.Forms.TextAlignment;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Droid.Extensions
{
	[global::Android.Runtime.Preserve(AllMembers = true)]
	public static class TextAlignmentExtensions
	{
		public static GravityFlags ToGravityFlags( this LayoutAlignment forms, bool expand )
		{
			GravityFlags result = forms switch
								  {
									  LayoutAlignment.Start  => GravityFlags.Start | GravityFlags.CenterVertical,
									  LayoutAlignment.Center => GravityFlags.CenterHorizontal | GravityFlags.CenterVertical,
									  LayoutAlignment.End    => GravityFlags.End | GravityFlags.CenterVertical,
									  _                      => GravityFlags.Fill
								  };

			if ( expand ) result |= GravityFlags.Fill;
			return result;
		}

		public static GravityFlags ToGravityFlags( this TextAlignment forms )
		{
			return forms switch
				   {
					   TextAlignment.Start  => GravityFlags.Left | GravityFlags.CenterVertical,
					   TextAlignment.Center => GravityFlags.CenterHorizontal | GravityFlags.CenterVertical,
					   TextAlignment.End    => GravityFlags.Right | GravityFlags.CenterVertical,
					   _                    => GravityFlags.Center | GravityFlags.CenterHorizontal
				   };
		}

		public static ATextAlignment ToAndroidTextAlignment( this TextAlignment forms )
		{
			return forms switch
				   {
					   TextAlignment.Start  => ATextAlignment.ViewStart,
					   TextAlignment.Center => ATextAlignment.Center,
					   TextAlignment.End    => ATextAlignment.ViewEnd,
					   _                    => ATextAlignment.Gravity
				   };
		}
	}
}
