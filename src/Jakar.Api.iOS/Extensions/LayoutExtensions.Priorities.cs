using UIKit;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public static partial class LayoutExtensions
	{
		public static void Priorities( this UIView view, in float hugging, in float compression )
		{
			view.HuggingPriority(hugging, UILayoutConstraintAxis.Horizontal, UILayoutConstraintAxis.Vertical);

			view.CompressionPriorities(compression, UILayoutConstraintAxis.Horizontal, UILayoutConstraintAxis.Vertical);
		}

		public static void CompressionPriorities( this UIView view, in float value, params UILayoutConstraintAxis[] directions )
		{
			foreach ( var axis in directions ) { view.SetContentCompressionResistancePriority(value, axis); }
		}

		public static void HuggingPriority( this UIView view, in float value, params UILayoutConstraintAxis[] directions )
		{
			foreach ( var axis in directions ) { view.SetContentHuggingPriority(value, axis); }
		}
	}
}
