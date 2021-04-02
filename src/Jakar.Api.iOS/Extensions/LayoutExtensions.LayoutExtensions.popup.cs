using System.Linq;
using UIKit;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.iOS.Extensions
{
	public static partial class LayoutExtensions
	{
		public static void SetPopup( this UITableViewCell cell, in UIView view ) => cell.InputView.AddFull(view);
		public static void RemovePopup( this UITableViewCell cell, UIView view ) => cell.InputView.Subviews.FirstOrDefault(item => item.Equals(view))?.RemoveFromSuperview();
		public static bool CanShowPopup( this UITableViewCell cell ) => cell.CanBecomeFirstResponder;
		public static bool ShowPopup( this UITableViewCell cell ) => cell.BecomeFirstResponder();
		public static bool CanHidePopup( this UITableViewCell cell ) => cell.CanResignFirstResponder;
		public static bool HidePopup( this UITableViewCell cell ) => cell.ResignFirstResponder();
	}
}