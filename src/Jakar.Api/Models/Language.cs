using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public class Language
	{
		public string DisplayName { get; init; }

		public string ShortName { get; init; }

		[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public SupportedLanguage Version { get; init; }

		[JsonProperty(nameof(Id), DefaultValueHandling = DefaultValueHandling.Include)]
		public long Id => (long) Version;

		public CultureInfo Info => new(ShortName);

		public Language() : this(string.Empty, string.Empty, SupportedLanguage.English) { }

		public Language( string name, string shortName, SupportedLanguage language )
		{
			DisplayName = name;
			ShortName = shortName;
			Version = language;
		}


		public static Language Arabic => new("عربى - Arabic", "ar", SupportedLanguage.Arabic);
		public static Language Chinese => new("中文 - Chinese (simplified)", "zh-Hans", SupportedLanguage.Chinese);
		public static Language Czech => new("čeština - Czech", "cs", SupportedLanguage.Czech);
		public static Language Dutch => new("Nederlands - Dutch", "nl", SupportedLanguage.Dutch);
		public static Language English => new("English", "en", SupportedLanguage.English);
		public static Language French => new("Français - French", "fr", SupportedLanguage.French);
		public static Language German => new("Deutsche - German", "de", SupportedLanguage.German);
		public static Language Japanese => new("日本語 - Japanese", "ja", SupportedLanguage.Japanese);
		public static Language Korean => new("한국어 - Korean", "ko", SupportedLanguage.Korean);
		public static Language Polish => new("Polskie - Polish", "pl", SupportedLanguage.Polish);
		public static Language Portuguese => new("Português - Portuguese", "pt", SupportedLanguage.Portuguese);
		public static Language Spanish => new("Español - Spanish", "es", SupportedLanguage.Spanish);
		public static Language Swedish => new("svenska - Swedish", "sv", SupportedLanguage.Swedish);
		public static Language Thai => new("ไทย - Thai", "th", SupportedLanguage.Thai);

		public static List<Language> All =>
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
}
