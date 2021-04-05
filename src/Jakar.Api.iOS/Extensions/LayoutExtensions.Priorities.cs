using System;
using Jakar.Api.Extensions;
using Jakar.Api.iOS.Enumerations;
using UIKit;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public static partial class LayoutExtensions
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
	}
}
