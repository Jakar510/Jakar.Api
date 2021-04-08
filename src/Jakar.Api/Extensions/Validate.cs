using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class Validate
	{
		public static string Demo { get; set; } = "DEMO";

		private static readonly Regex _emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

		public static string FormatNumber<T>( this T number, int maxDecimals = 4 ) => Regex.Replace(string.Format(LanguageApi.Current.CultureInfo, $"{{0:n{maxDecimals}}}", number),
																									$"[{LanguageApi.Current.CultureInfo.NumberFormat.NumberDecimalSeparator}]?0+$",
																									"");


		public static bool IsIPv4( this string ipString )
		{
			if ( string.IsNullOrWhiteSpace(ipString) ) return false;

			string[] splitValues = ipString.Split('.');
			return splitValues.Length == 4 && splitValues.All(IsInteger) && ParseIPv4(ipString) != null;
		}

		public static IPAddress? ParseIPv4( this string ipString ) => IPAddress.TryParse(ipString!, out IPAddress address)
																		  ? address
																		  : null;


		public static bool IsWebAddress( this string addressString )
		{
			if ( string.IsNullOrWhiteSpace(addressString) ) return false;

			Uri? uriResult = ParseWebAddress(addressString);
			return uriResult != null && ( uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps );
		}

		public static Uri? ParseWebAddress( this string addressString ) => Uri.TryCreate(addressString, UriKind.Absolute, out Uri uriResult)
																			   ? uriResult
																			   : null;


		public static bool IsEmailAddress( this string email ) => ValidateEmail(email) && IsValidEmail(email);

		public static bool ValidateEmail( this string email )
		{
			if ( string.IsNullOrWhiteSpace(email) ) return false;

			return email.Contains("@", StringComparison.OrdinalIgnoreCase) && email.Contains(".", StringComparison.OrdinalIgnoreCase) &&
				   ( !email.Contains(",", StringComparison.OrdinalIgnoreCase) || !email.Contains("~", StringComparison.OrdinalIgnoreCase) );
		}

		private static bool IsValidEmail( this string email ) => _emailRegex.IsMatch(email) && ParseEmail(email) != null;

		public static MailAddress? ParseEmail( this string email )
		{
			try { return new MailAddress(email); }
			catch ( FormatException ) { return null; }
		}


		public static bool IsValidPort( this string s ) => int.TryParse(s, out int port) && port.IsValidPort();
		public static bool IsValidPort( this int port ) => port > 0 && port <= 65535;

		public static bool IsDouble( this string argsNewTextValue ) => double.TryParse(argsNewTextValue, out double _);
		public static bool IsInteger( this string argsNewTextValue ) => int.TryParse(argsNewTextValue, out int _);

		internal static bool IsDemo( this string item )
		{
			if ( string.IsNullOrWhiteSpace(item) ) return false;

			item = item.ToLower(LanguageApi.Current.CultureInfo);
			return item == Demo || item == "demo";
		}


		public static T ThrowIfNull<T>( T arg, string name, [CallerMemberName] string caller = "" ) => arg ?? throw new ArgumentNullException(name, caller);

		public static string ThrowIfNull( string arg, string name, [CallerMemberName] string caller = "" ) => string.IsNullOrWhiteSpace(arg)
																												  ? throw new ArgumentNullException(name, caller)
																												  : arg;
	}
}
