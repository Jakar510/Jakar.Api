using Jakar.Extensions.General;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class ColorExtensions
	{
		public static Color ToXfColor( this System.Drawing.Color color ) => new(color.R, color.G, color.B, color.A);

		public static System.Drawing.Color ToDrawingColor( this Color color ) => System.Drawing.Color.FromArgb(color.A.ToInt(), color.R.ToInt(), color.G.ToInt(), color.B.ToInt());
	}
}
