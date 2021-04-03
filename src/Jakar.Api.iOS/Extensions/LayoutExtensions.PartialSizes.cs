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
		public static IList<NSLayoutConstraint> PartOfParent( this UIView view,
															  in UIView parent,
															  in nfloat leftFactor,
															  in nfloat rightFactor,
															  in nfloat topFactor,
															  in nfloat bottomFactor
		)
		{
			IList<NSLayoutConstraint> results = view.HorizontalPartOfParent(parent, leftFactor, rightFactor);
			view.VerticalPartOfParent(parent, topFactor, bottomFactor).ForEach(results.Add);
			return results;
		}

		public static IList<NSLayoutConstraint> HorizontalPartOfParent( this UIView view, in UIView parent, in nfloat leftFactor, in nfloat rightFactor )
		{
			var constraints = new List<NSLayoutConstraint>
							  {
								  view.LeftOfParent(parent, leftFactor),
								  view.RightOfParent(parent, rightFactor),
								  view.WidthOfParent(parent, leftFactor, rightFactor)
							  };

			return constraints;
		}

		public static IList<NSLayoutConstraint> VerticalPartOfParent( this UIView view, in UIView parent, in nfloat topFactor, in nfloat bottomFactor )
		{
			var constraints = new List<NSLayoutConstraint>
							  {
								  view.TopOfParent(parent, topFactor),
								  view.BottomOfParent(parent, bottomFactor)
							  };

			return constraints;
		}


		public static NSLayoutConstraint WidthOfParent( this UIView view, in UIView parent, in nfloat leftFactor, in nfloat rightFactor ) =>
			view.WidthOfParent(parent, Math.Abs(leftFactor - rightFactor).ToNFloat());

		public static NSLayoutConstraint WidthOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.WidthAnchor.ConstraintEqualTo(parent.WidthAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint LeftOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.LeftAnchor.ConstraintEqualTo(parent.LeftAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint RightOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.RightAnchor.ConstraintEqualTo(parent.RightAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint HeightOfParent( this UIView view, in UIView parent, in nfloat topFactor, in nfloat bottomFactor ) =>
			view.HeightOfParent(parent, Math.Abs(topFactor - bottomFactor).ToNFloat());

		public static NSLayoutConstraint HeightOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.HeightAnchor.ConstraintEqualTo(parent.HeightAnchor, factor);
			anchor.Active = true;
			return anchor;
		}


		public static NSLayoutConstraint TopOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.TopAnchor.ConstraintEqualTo(parent.TopAnchor, factor);
			anchor.Active = true;
			return anchor;
		}

		public static NSLayoutConstraint BottomOfParent( this UIView view, in UIView parent, in nfloat factor )
		{
			NSLayoutConstraint anchor = view.BottomAnchor.ConstraintEqualTo(parent.BottomAnchor, factor);
			anchor.Active = true;
			return anchor;
		}
	}
}
