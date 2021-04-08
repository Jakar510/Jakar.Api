using System;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms.Internals;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public static partial class LayoutExtensions
	{
	#region WidthOf

		public static NSLayoutConstraint WidthOf( this UIView view, in UIView other, in nfloat leftFactor, in nfloat rightFactor ) =>
			view.WidthOf(other, Math.Abs(leftFactor - rightFactor).ToNFloat());

		public static NSLayoutConstraint WidthOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.WidthAnchor.ConstraintEqualTo(other.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint MinimumWidthOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.WidthAnchor.ConstraintGreaterThanOrEqualTo(other.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint MaximumWidthOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.WidthAnchor.ConstraintLessThanOrEqualTo(other.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

	#endregion


	#region HeightOf

		public static NSLayoutConstraint HeightOf( this UIView view, in UIView other, in nfloat topFactor, in nfloat bottomFactor ) =>
			view.HeightOf(other, Math.Abs(topFactor - bottomFactor).ToNFloat());

		public static NSLayoutConstraint HeightOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.HeightAnchor.ConstraintEqualTo(other.HeightAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint MinimumHeightOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.HeightAnchor.ConstraintGreaterThanOrEqualTo(other.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint MaximumHeightOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.HeightAnchor.ConstraintLessThanOrEqualTo(other.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

	#endregion


		public static NSLayoutConstraint LeftOf( this UIView view, in UIView other )
		{
			NSLayoutConstraint anchor = view.LeftAnchor.ConstraintEqualTo(other.RightAnchor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint LeftOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.LeftAnchor.ConstraintEqualTo(other.RightAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint RightOf( this UIView view, in UIView other )
		{
			NSLayoutConstraint anchor = view.RightAnchor.ConstraintEqualTo(other.LeftAnchor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint RightOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.RightAnchor.ConstraintEqualTo(other.RightAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint BelowOf( this UIView view, in UIView other )
		{
			NSLayoutConstraint anchor = view.TopAnchor.ConstraintEqualTo(other.BottomAnchor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint BelowOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.TopAnchor.ConstraintEqualTo(other.BottomAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint AboveOf( this UIView view, in UIView other )
		{
			NSLayoutConstraint anchor = view.BottomAnchor.ConstraintEqualTo(other.TopAnchor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint AboveOf( this UIView view, in UIView other, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.BottomAnchor.ConstraintEqualTo(other.TopAnchor, factor);
			anchor.Active = true;
			return anchor;
		}
	}
}
