using System;
using System.Collections.Generic;
using Jakar.Api.Exceptions.General;
using Jakar.Api.Exceptions.Networking;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	public class BaseResourceDictionary<TThemedKey, TKey> : ResourceDictionary // where TThemedKey : Enum where TEnum : Enum
	{
		public BaseResourceDictionary() { }


		public static string GetKey( in OSAppTheme theme, in TThemedKey key ) => $"{theme}{key}";


		public void Add( in TThemedKey key, in Style light, in Style dark )
		{
			Add(OSAppTheme.Light, key, light);
			Add(OSAppTheme.Dark,  key, dark);
		}

		public void Add( in TThemedKey key, in Style style )
		{
			Add(OSAppTheme.Light, key, style);
			Add(OSAppTheme.Dark,  key, style);
		}

		public void Add<TValue>( in OSAppTheme theme, in TThemedKey key, TValue value ) { Add(GetKey(theme, key), value); }

		public TValue Get<TValue>( in OSAppTheme theme, in TThemedKey key )
		{
			if ( !TryGetValue(GetKey(theme, key), out object value) ) { throw new KeyNotFoundException(nameof(GetKey)); }

			if ( value is TValue item ) { return item; }

			throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
		}


		public void Add<TValue>( in TKey key, TValue value )
		{
			if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

			Add(key.ToString(), value);
		}

		public TValue Get<TValue>( in TKey key )
		{
			if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

			if ( !TryGetValue(key.ToString(), out object value) ) { throw new KeyNotFoundException(nameof(key)); }

			if ( value is TValue item ) { return item; }

			throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
		}


		public void BindAppTheme( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) => BindAppTheme<Color>(bindable, property, key);

		public void BindAppTheme<TBinding>( in BindableObject bindable, in BindableProperty property, in TThemedKey key )
		{
			bindable.SetOnAppTheme(property, Get<TBinding>(OSAppTheme.Light, key), Get<TBinding>(OSAppTheme.Dark, key));
		}
	}



	// public enum ResourceName
	// {
	// 	ShellForegroundColor,
	// 	ShellBackgroundColor,
	// 	ShellNavigationBarColor,
	// 	ShellDisabledColor,
	// 	ShellUnselectedColor,
	// 	FlyOutBorderColor,
	//
	// 	PageTitleFontSize,
	// 	TitleFontSize,
	// 	HeaderFontSize,
	// 	DescriptionFontSize,
	// 	HintFontSize,
	// 	ValueFontSize,
	//
	// 	InFocusColor,
	// }
	// public enum ThemedResourceName
	// {
	// 	TextColor,
	// 	BackgroundColor,
	// 	PageBackgroundColor,
	// 	AccentColor,
	// 	AcceptColor,
	// 	CancelColor,
	// 	SeparatorColor,
	// 	PlaceholderColor,
	//
	// 	HeaderTextColor,
	// 	HeaderBackgroundColor,
	//
	// 	FooterTextColor,
	// 	FooterBackgroundColor,
	//
	// 	ButtonBorderColor,
	// 	ButtonBackgroundColor,
	// 	ButtonForegroundColor,
	// }
}
