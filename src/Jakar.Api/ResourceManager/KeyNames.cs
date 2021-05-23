using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.ResourceManager
{
	[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
	public class KeyNames
	{
		public enum FontSize
		{
			PageTitle,
			Header,
			Title,
			Description,
			Hint,
			Value,
			Misc,
		}



		public enum ShellColor
		{
			Foreground,
			Background,
			NavigationBar,
			Disabled,
			Unselected,
			FlyOutBorder,
		}



		public enum ThemedColor
		{
			Text,
			Background,
			PageBackground,
			Accent, // i.e. CheckBox
			Valid,
			Invalid,
			InFocus,
		}



		public enum ListView
		{
			BackgroundColor,
			SeparatorColor, // i.e. ListView
		}



		public enum Button
		{
			BorderColor,
			TextColor,
			BackgroundColor,
			FontSize,
			FontAttributes,
			FontFamily,
		}



		public enum Entry
		{
			AccentColor,
			TextColor,
			PlaceholderColor,
			BackgroundColor,
			FontSize,
			FontAttributes,
			FontFamily,
		}



		public enum Label
		{
			TextColor,
			BackgroundColor,
			FontSize,
			FontAttributes,
			FontFamily,
		}



		public enum Header
		{
			TextColor,
			BackgroundColor,
			FontSize,
			FontAttributes,
			FontFamily,
		}



		public enum Footer
		{
			TextColor,
			BackgroundColor,
			FontSize,
			FontAttributes,
			FontFamily,
		}
	}
}
