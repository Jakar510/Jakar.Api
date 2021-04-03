using UIKit;
using Xamarin.Forms;


#pragma warning disable 1591

namespace Jakar.Api.iOS.Extensions
{
	[Foundation.Preserve(AllMembers = true)]
	public static class ThicknessExtensions
	{
		// ReSharper disable once InconsistentNaming
		public static UIEdgeInsets ToUIEdgeInsets( this Thickness forms ) => new(forms.Top.ToNFloat(), forms.Left.ToNFloat(), forms.Bottom.ToNFloat(), forms.Right.ToNFloat());
	}
}
