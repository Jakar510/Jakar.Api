using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms.Internals;
#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public static partial class LayoutExtensions
	{
		public static void AddFull( this UIStackView parent, UIView view )
		{
			parent.AddArrangedSubview(view);
			view.SetBounds(parent);
			view.UpdateConstraintsIfNeeded();
		}
		public static void AddFull( this UIView parent, UIView view )
		{
			parent.AddSubview(view);
			view.SetBounds(parent);
			view.UpdateConstraintsIfNeeded();
		}
		public static void AddFull( this UITableViewCell cell, in UIView view ) => cell.ContentView.AddFull(view);
		public static void SetAccessory( this UITableViewCell cell, in UIView view ) => cell.EditingAccessoryView = cell.AccessoryView = view;


		public static void LeftExtended( this UIView view, in UIView parent, in UIView right ) => view.SetBounds(parent.TopAnchor, parent.BottomAnchor, parent.LeftAnchor, right.LeftAnchor);
		public static void RightExtended( this UIView view, in UIView parent, in UIView left ) => view.SetBounds(parent.TopAnchor, parent.BottomAnchor, left.RightAnchor, parent.RightAnchor);


		public static void InBetween( this UIView view, in UIView parent, in UIView left, in UIView right ) => view.SetBounds(parent.TopAnchor, parent.BottomAnchor, right.LeftAnchor, left.RightAnchor);


		public static void SetBounds( this UIView view, in UIView parent, in bool translate = false ) => view.SetBounds(parent.TopAnchor, parent.BottomAnchor, parent.LeftAnchor, parent.RightAnchor, translate);
		public static void SetBounds( this UIView view, in NSLayoutYAxisAnchor top, in NSLayoutYAxisAnchor bottom, in NSLayoutXAxisAnchor left, in NSLayoutXAxisAnchor right, in bool translate = false )
		{
			view.TranslatesAutoresizingMaskIntoConstraints = translate;
			view.SetWidthBounds(left, right);
			view.SetHeightBounds(top, bottom);
		}
		public static void SetWidthBounds( this UIView view, in NSLayoutXAxisAnchor left, in NSLayoutXAxisAnchor right )
		{
			view.LeftAnchor.ConstraintEqualTo(left).Active = true;

			view.RightAnchor.ConstraintEqualTo(right).Active = true;
		}
		public static void SetHeightBounds( this UIView view, in NSLayoutYAxisAnchor top, in NSLayoutYAxisAnchor bottom )
		{
			view.TopAnchor.ConstraintEqualTo(top).Active = true;

			view.BottomAnchor.ConstraintEqualTo(bottom).Active = true;
		}
	}
}