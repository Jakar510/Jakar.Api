using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.ResourceManager
{
	[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
	public class KeyNames
	{
		internal static void Test()
		{
			var manager = new ResourceDictionaryManager();
			manager.Add(OSAppTheme.Dark, ThemedColor.Accent, Color.Wheat);
			manager.Add(OSAppTheme.Light, ThemedColor.Accent, Color.Black);
			manager.Add(OSAppTheme.Light, FontSize.PageTitle, 25);
			manager.Add(OSAppTheme.Light, FontSize.Title, 20);
		}



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



		public enum Style
		{
			FontSize,
			FontAttributes,
			FontFamily,
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
			Accent,
			Accept,
			Cancel,
			Separator,
			Placeholder,
			InFocus,
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
