using System;
using Jakar.Api.iOS.Enumerations;
using Jakar.Extensions.General;
using UIKit;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions.Layout
{
	public static class LayoutPriorityExtensions
	{
		public static void Priorities( this UIView view, in LayoutPriority hugging, in LayoutPriority compression ) => view.Priorities(hugging.ToFloat(), compression.ToFloat());

		public static void Priorities( this UIView view, in float hugging, in float compression )
		{
			view.HuggingPriority(hugging, UILayoutConstraintAxis.Horizontal, UILayoutConstraintAxis.Vertical);

			view.CompressionPriorities(compression, UILayoutConstraintAxis.Horizontal, UILayoutConstraintAxis.Vertical);
		}


		public static void CompressionPriorities( this UIView view, in LayoutPriority value, params UILayoutConstraintAxis[] directions ) => view.CompressionPriorities(value.ToFloat(), directions);

		public static void CompressionPriorities( this UIView view, in float value, params UILayoutConstraintAxis[] directions )
		{
			foreach ( UILayoutConstraintAxis axis in directions ) { view.SetContentCompressionResistancePriority(value, axis); }
		}


		public static void HuggingPriority( this UIView view, in LayoutPriority value, params UILayoutConstraintAxis[] directions ) => view.HuggingPriority(value.ToFloat(), directions);

		public static void HuggingPriority( this UIView view, in float value, params UILayoutConstraintAxis[] directions )
		{
			foreach ( UILayoutConstraintAxis axis in directions ) { view.SetContentHuggingPriority(value, axis); }
		}


		public static LayoutPriority ToLayoutPriority( this UILayoutPriority priority ) => priority switch
																						   {
																							   UILayoutPriority.Required => LayoutPriority.Required,
																							   UILayoutPriority.DefaultHigh => LayoutPriority.High,
																							   UILayoutPriority.DefaultLow => LayoutPriority.Low,
																							   UILayoutPriority.FittingSizeLevel => LayoutPriority.Lowest,
																							   UILayoutPriority.DragThatCanResizeScene => LayoutPriority.AboveAverage,
																							   UILayoutPriority.SceneSizeStayPut => LayoutPriority.Average,
																							   UILayoutPriority.DragThatCannotResizeScene => LayoutPriority.BelowAverage,
																							   _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
																						   };

		public static UILayoutPriority ToLayoutPriority( this LayoutPriority priority ) => priority switch
																						   {
																							   LayoutPriority.Required     => UILayoutPriority.Required,
																							   LayoutPriority.Highest      => UILayoutPriority.Required,
																							   LayoutPriority.High         => UILayoutPriority.DefaultHigh,
																							   LayoutPriority.Lowest       => UILayoutPriority.FittingSizeLevel,
																							   LayoutPriority.AboveAverage => UILayoutPriority.DragThatCanResizeScene,
																							   LayoutPriority.Average      => UILayoutPriority.SceneSizeStayPut,
																							   LayoutPriority.BelowAverage => UILayoutPriority.DragThatCannotResizeScene,
																							   LayoutPriority.Low          => UILayoutPriority.DefaultLow,
																							   LayoutPriority.VeryLow      => UILayoutPriority.FittingSizeLevel,
																							   LayoutPriority.Minimum      => UILayoutPriority.FittingSizeLevel,
																							   LayoutPriority.Zero         => 0,
																							   _                           => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
																						   };
	}
}
