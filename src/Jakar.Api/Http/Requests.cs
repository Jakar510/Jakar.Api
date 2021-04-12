using System;
using System.Linq;
using Jakar.Api.Exceptions.Networking;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class Requests
	{
		public const int DEFAULT_TIMEOUT = 10 * 1000;
		public const int SHORT_TIMEOUT = 2000;


		public static T VerifyReply<T>( string reply )
		{
			if ( string.IsNullOrWhiteSpace(reply) ) { throw new EmptyServerResponseException(nameof(reply)); }

			return reply.FromJson<T>();
		}


		private static readonly IBaseUrl _baseUrl = DependencyService.Get<IBaseUrl>();
		public static Uri BaseUrl => _baseUrl.GetUri();
		public static string GetLocalFilePath( string fileName ) => $"{BaseUrl}{fileName}";


		public static Uri GetUri( this string baseUri, params string[] parameters ) => new Uri(baseUri, UriKind.Absolute).GetUri(parameters);

		public static Uri GetUri( this Uri baseUri, params string[] parameters )
		{
			if ( baseUri is null ) { throw new ArgumentNullException(nameof(baseUri)); }

			return parameters.Length <= 0
					   ? baseUri
					   : new Uri(baseUri, GetParameters(parameters));
		}

		public static string GetParameters( params string[] parameters )
		{
			if ( parameters.Length <= 0 )
				return string.Empty;

			string result = parameters.Aggregate("?",
												 CondenseParameters);

			result = result.Remove(result.Length - 1);

			return result;
		}

		private static string CondenseParameters( string current, string element )
		{
			return current + ( string.IsNullOrWhiteSpace(element)
								   ? ""
								   : $"{element}&" );
		}
	}
}
