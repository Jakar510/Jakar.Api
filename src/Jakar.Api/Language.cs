using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	public class Language
	{
		public enum SupportedLanguage
		{
			English,
			Spanish,
			French,
			Swedish,
			German,
			Chinese,
			Polish,
			Thai,
			Japanese,
			Czech,
			Portuguese,
			Dutch,
			Korean,
			Arabic,
		}



		public class Item
		{
			public string DisplayName { get; init; }

			public string ShortName { get; init; }

			[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
			public SupportedLanguage Version { get; init; }

			[JsonProperty(nameof(Id), DefaultValueHandling = DefaultValueHandling.Include)]
			public long Id => (long) Version;

			public CultureInfo Info => new(ShortName);

			public Item() : this(string.Empty, string.Empty, SupportedLanguage.English) { }

			public Item( string name, string shortName, SupportedLanguage language )
			{
				DisplayName = name;
				ShortName = shortName;
				Version = language;
			}


			public static Item Arabic => new("عربى - Arabic", "ar", SupportedLanguage.Arabic);
			public static Item Chinese => new("中文 - Chinese (simplified)", "zh-Hans", SupportedLanguage.Chinese);
			public static Item Czech => new("čeština - Czech", "cs", SupportedLanguage.Czech);
			public static Item Dutch => new("Nederlands - Dutch", "nl", SupportedLanguage.Dutch);
			public static Item English => new("English", "en", SupportedLanguage.English);
			public static Item French => new("Français - French", "fr", SupportedLanguage.French);
			public static Item German => new("Deutsche - German", "de", SupportedLanguage.German);
			public static Item Japanese => new("日本語 - Japanese", "ja", SupportedLanguage.Japanese);
			public static Item Korean => new("한국어 - Korean", "ko", SupportedLanguage.Korean);
			public static Item Polish => new("Polskie - Polish", "pl", SupportedLanguage.Polish);
			public static Item Portuguese => new("Português - Portuguese", "pt", SupportedLanguage.Portuguese);
			public static Item Spanish => new("Español - Spanish", "es", SupportedLanguage.Spanish);
			public static Item Swedish => new("svenska - Swedish", "sv", SupportedLanguage.Swedish);
			public static Item Thai => new("ไทย - Thai", "th", SupportedLanguage.Thai);

			public static List<Item> ALL =>
				new()
				{
					Arabic,
					Chinese,
					Czech,
					Dutch,
					English,
					French,
					German,
					Japanese,
					Korean,
					Polish,
					Portuguese,
					Spanish,
					Swedish,
					Thai
				};
		}



		internal static ObservableCollection<Item> Languages { get; } = new(Item.ALL);


		internal SupportedLanguage _CurrentLangVersion = (SupportedLanguage) Preferences.Get(nameof(CurrentLangVersion), (int) SupportedLanguage.English);

		internal SupportedLanguage CurrentLangVersion
		{
			get => _CurrentLangVersion;
			private set
			{
				try
				{
					_CurrentLangVersion = value;
					Preferences.Set(nameof(CurrentLangVersion), (int) _CurrentLangVersion);
				}
				catch ( Exception ex ) { Debug.Current.HandleException(ex); }
			}
		}


		private string _selectedLanguageName = Preferences.Get(nameof(SelectedLanguageName), string.Empty);

		public string SelectedLanguageName
		{
			get => _selectedLanguageName;
			private set
			{
				_selectedLanguageName = value;
				Preferences.Set(nameof(SelectedLanguageName), _selectedLanguageName);
			}
		}


		private Item? _selectedLanguage;

		public Item SelectedLanguage
		{
			get => _selectedLanguage ?? throw new NullReferenceException(nameof(_selectedLanguage));
			set
			{
				_selectedLanguage = value;
				if ( _selectedLanguage is null ) return;

				CultureInfo = _selectedLanguage.Info;
				CurrentLangVersion = _selectedLanguage.Version;
				SelectedLanguageName = _selectedLanguage.DisplayName;
			}
		}

		//internal Language SelectedLanguage { get; set; }


		private CultureInfo? _currentCultureInfo;

		public virtual CultureInfo CultureInfo
		{
			get => _currentCultureInfo ?? throw new NullReferenceException(nameof(_currentCultureInfo));
			protected set
			{
				_currentCultureInfo = value;
				Thread.CurrentThread.CurrentCulture = value;
			}
		}


		public Language()
		{
			string id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

			// ReSharper disable once VirtualMemberCallInConstructor
			CultureInfo = new CultureInfo(id);

			try { SelectedLanguage = Languages.First(language => language.ShortName == id); }
			catch ( Exception ) { SelectedLanguage = Languages.First(language => language.ShortName == "en"); }
		}


		public static Language Current => _Service.Value;
		private static Lazy<Language> _Service { get; } = new(Create, false);
		private static Language Create() => new();
	}
}
