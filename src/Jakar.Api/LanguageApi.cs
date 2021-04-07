using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	[SuppressMessage("ReSharper", "StringLiteralTypo")]
	public class LanguageApi
	{
		internal static ObservableCollection<Language> Languages { get; } = new(Language.All);


		internal SupportedLanguage currentLangVersion = (SupportedLanguage) Preferences.Get(nameof(CurrentLangVersion), (int) SupportedLanguage.English);

		internal SupportedLanguage CurrentLangVersion
		{
			get => currentLangVersion;
			private set
			{
				currentLangVersion = value;
				Preferences.Set(nameof(CurrentLangVersion), (int) currentLangVersion);
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


		private Language? _selectedLanguage;

		public Language SelectedLanguage
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


		public LanguageApi()
		{
			string id = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

			// ReSharper disable once VirtualMemberCallInConstructor
			CultureInfo = new CultureInfo(id);

			try { SelectedLanguage = Languages.First(language => language.ShortName == id); }
			catch ( Exception ) { SelectedLanguage = Languages.First(language => language.ShortName == "en"); }
		}


		public static LanguageApi Current => _Service.Value;
		private static Lazy<LanguageApi> _Service { get; } = new(Create, false);
		private static LanguageApi Create() => new();
	}
}
