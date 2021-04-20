using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jakar.Api.Exceptions.General;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.ResourceManager
{
	/// <summary>
	/// Recommended class to use for the default implementation: <see cref="KeyNames"/>
	/// If you want to modify the implementation, override <see cref="GetKey{TThemedKey}"/>
	/// </summary>
	public class ResourceDictionaryManager
	{
		protected static ResourceDictionary _Current => Application.Current.Resources;
		public ResourceDictionaryManager() { }


		public static string GetFullKey<TThemedKey>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum  => $"{theme}.{typeof(TThemedKey).FullName}.{key}";
		public static string GetShortKey<TThemedKey>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum  => $"{theme}{key}";

		/// <summary>
		/// Default implementation uses <see cref="GetFullKey{TThemedKey}"/>.
		/// You may override this to use <see cref="GetShortKey{TThemedKey}"/> or which ever pattern you wish.
		/// </summary>
		/// <typeparam name="TThemedKey"></typeparam>
		/// <param name="theme"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual string GetKey<TThemedKey>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum  => GetFullKey(theme, key);


		public void Add<TThemedKey>( in TThemedKey key, in Style light, in Style dark ) where TThemedKey : Enum 
		{
			Add(OSAppTheme.Light, key, light);
			Add(OSAppTheme.Dark, key, dark);
		}

		public void Add<TThemedKey>( in TThemedKey key, in Style style ) where TThemedKey : Enum 
		{
			Add(OSAppTheme.Light, key, style);
			Add(OSAppTheme.Dark, key, style);
		}

		public void Add<TThemedKey, TValue>( in OSAppTheme theme, in TThemedKey key, TValue value ) where TThemedKey : Enum { _Current.Add(GetKey(theme, key), value); }

		public TValue Get<TThemedKey, TValue>( in OSAppTheme theme, in TThemedKey key ) where TThemedKey : Enum
		{
			if ( !_Current.TryGetValue(GetKey(theme, key), out object value) ) { throw new KeyNotFoundException(nameof(GetKey)); }

			if ( value is TValue item ) { return item; }

			throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
		}


		public void Add<TKey, TValue>( in TKey key, TValue value ) where TKey : Enum
		{
			if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

			_Current.Add(key.ToString(), value);
		}

		public TValue Get<TKey, TValue>( in TKey key ) where TKey : Enum
		{
			if ( key is null ) { throw new ArgumentNullException(nameof(key)); }

			if ( !_Current.TryGetValue(key.ToString(), out object value) ) { throw new KeyNotFoundException(nameof(key)); }

			if ( value is TValue item ) { return item; }

			throw new ExpectedValueTypeException(nameof(item), value, typeof(TValue));
		}


		public void BindAppTheme<TThemedKey>( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) where TThemedKey : Enum =>
			BindAppTheme<Color, TThemedKey>(bindable, property, key);

		public void BindAppTheme<TValue, TThemedKey>( in BindableObject bindable, in BindableProperty property, in TThemedKey key ) where TThemedKey : Enum
		{
			bindable.SetOnAppTheme(property, Get<TThemedKey, TValue>(OSAppTheme.Light, key), Get<TThemedKey, TValue>(OSAppTheme.Dark, key));
		}
	}
}
