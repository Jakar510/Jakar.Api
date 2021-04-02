using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Exceptions.Networking;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Xamarin.Forms;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
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


		public static Uri GetUri( string baseUri, params string[] parameters ) => GetUri(new Uri(baseUri, UriKind.Absolute), parameters);
		public static Uri GetUri( Uri baseUri, params string[] parameters )
		{
			if ( baseUri is null )
				throw new ArgumentNullException(nameof(baseUri));

			return parameters.Length <= 0 ? baseUri : new Uri(baseUri, GetParameters(parameters));
		}
		public static string GetParameters( params string[] parameters )
		{
			if ( parameters.Length <= 0 )
				return string.Empty;

			string result = parameters.Aggregate("?", ( current, element ) => current + ( string.IsNullOrWhiteSpace(element) ? "" : $"{element}&" ));
			result = result.Remove(result.Length - 1);

			return result;
		}


		public static class Pings
		{
			// internal static async Task<bool> TryPing( CancellationToken token ) => await TryPing(Activity.Current.User.Config.GetSiteAddress("ping"), SHORT_TIMEOUT, token).ConfigureAwait(true);
			internal static async Task<bool> TryPing( Uri url, int timeout, CancellationToken token )
			{
				try
				{
					const string PAYLOAD = @"{ ""ping"":"""" }";
					string reply = await Ping(url, PAYLOAD, timeout, token).ConfigureAwait(true);

					return !string.IsNullOrWhiteSpace(reply);
				}
				catch ( TimeoutException ) { return false; }
			}


			// internal static async Task<string> Ping( CancellationToken token ) => await Ping(SHORT_TIMEOUT, token).ConfigureAwait(true);
			// internal static async Task<string> Ping( int timeout, CancellationToken token ) => await Ping(Activity.Current.User.Config.GetSiteAddress("ping"), @"{ ""ping"":"""" }", timeout, token).ConfigureAwait(true);
			internal static async Task<string> Ping( Uri url, string payload, int timeout, CancellationToken token ) => await Posts.PostJson(url, payload, timeout, token).ConfigureAwait(true);


			internal static async Task<string> Ping( Uri url, CancellationToken token ) => await Ping(url, 2000, token).ConfigureAwait(true);
			internal static async Task<string> Ping( Uri url, int timeout, CancellationToken token ) => await Gets.TryGet(Gets.Get, url, timeout, token).ConfigureAwait(true);
		}


		public static class Gets
		{
			public static async Task<string> Get( string baseUrl, CancellationToken token, params string[] parameters ) => await Get(new Uri(baseUrl, UriKind.Absolute), 10000, token, parameters).ConfigureAwait(true);
			public static async Task<string> Get( string baseUrl, int timeout, CancellationToken token, params string[] parameters ) => await Get(new Uri(baseUrl, UriKind.Absolute), timeout, token, parameters).ConfigureAwait(true);
			public static async Task<string> Get( Uri baseUrl, CancellationToken token, params string[] parameters ) => await Get(GetUri(baseUrl, parameters), 10000, token).ConfigureAwait(true);
			public static async Task<string> Get( Uri baseUrl, int timeout, CancellationToken token, params string[] parameters ) => await Get(GetUri(baseUrl, parameters), timeout, token).ConfigureAwait(true);
			public static async Task<string> Get( Uri uri, CancellationToken token )
			{
				string result = await Get(uri, 10000, token).ConfigureAwait(true);

				return result.Trim();
			}
			public static async Task<string> Get( Uri uri, int timeout, CancellationToken token )
			{
				// https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx

				var req = WebRequest.Create(uri);
				req.Timeout = timeout;
				req.Method = "GET";
				req.ContentType = MediaTypeNames.URL_ENCODED_CONTENT;
				req.Headers[HttpRequestHeader.ContentEncoding] = Encoding.UTF8.ToString();

				using WebResponse resp = await req.GetResponseAsync(token).ConfigureAwait(true);

				await using Stream? stream = resp.GetResponseStream();
				using var sr = new StreamReader(stream ?? throw new Exception());
				string result = await sr.ReadToEndAsync().ConfigureAwait(true);

				return result.Trim();
			}
			internal static async Task<string> TryGet( Func<Uri, int, CancellationToken, Task<string>> func, Uri url, int timeout, CancellationToken token )
			{
				try { return await func(url, timeout, token).ConfigureAwait(true); }
				catch ( WebException we )
				{
					//WebExceptionStatus status = we.Status;
					Exception? e = we.Status switch
								   {
									   //WebExceptionStatus.CacheEntryNotFound => new ,
									   WebExceptionStatus.ConnectFailure => new ConnectFailureException(we.Message, we, token),
									   WebExceptionStatus.ConnectionClosed => new ConnectionClosedException(we.Message, we, token),
									   WebExceptionStatus.KeepAliveFailure => new KeepAliveFailureException(we.Message, we, token),
									   //WebExceptionStatus.MessageLengthLimitExceeded => new ,
									   WebExceptionStatus.NameResolutionFailure => new NameResolutionException(we.Message, we, token),
									   //WebExceptionStatus.Pending => new ,
									   //WebExceptionStatus.PipelineFailure => new ,
									   //WebExceptionStatus.ProtocolError => new ,
									   //WebExceptionStatus.ProxyNameResolutionFailure => new ,
									   WebExceptionStatus.ReceiveFailure => new ReceiveFailureException(we.Message, we, token),
									   WebExceptionStatus.RequestCanceled => new RequestAbortedException(we.Message, we, token),
									   //WebExceptionStatus.RequestProhibitedByCachePolicy => new ,
									   //WebExceptionStatus.RequestProhibitedByProxy => new ,
									   //WebExceptionStatus.SecureChannelFailure => new ,
									   WebExceptionStatus.SendFailure => new SendFailureException(we.Message, we, token),
									   //WebExceptionStatus.ServerProtocolViolation => new ,
									   WebExceptionStatus.Success => null,
									   WebExceptionStatus.Timeout => new TimeoutException(we.Message, we),
									   //WebExceptionStatus.TrustFailure => new ,
									   //WebExceptionStatus.UnknownError => new ,
									   _ => null
								   };

					if ( e != null )
						throw e;

					throw;
				}
			}
		}


		public static class Posts
		{
			// https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx
			// https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads


			public static async Task<string> Post( string baseUri, CancellationToken token, params string[] parameters ) => await Post(new Uri(baseUri), token, parameters).ConfigureAwait(true);
			public static async Task<string> Post( Uri baseUri, CancellationToken token, params string[] parameters )
			{
				string payload = parameters.Aggregate(string.Empty, ( current, element ) => current + ( string.IsNullOrWhiteSpace(element) ? "" : $"{element}&" ));
				payload = payload.Remove(payload.Length - 1);

				return await Post(baseUri, payload, token).ConfigureAwait(true);
			}
			public static async Task<string> Post( Uri url, string payload, CancellationToken token ) => await Post(url, payload, MediaTypeNames.URL_ENCODED_CONTENT, token).ConfigureAwait(true);
			public static async Task<string> Post( Uri url, string payload, string contentType, CancellationToken token ) => await Post(url, payload, contentType, DEFAULT_TIMEOUT, token).ConfigureAwait(true);
			public static async Task<string> Post( Uri url, string payload, string contentType, int timeout, CancellationToken token ) => await TryPost(Post, url, payload, contentType, timeout, token).ConfigureAwait(true);


			public static async Task<string> PostJson( Uri url, string payload, CancellationToken token )
			{
				// Activity.Current.Settings.OutgoingActivityText = payload;

				return await PostJson(url, payload, DEFAULT_TIMEOUT, token).ConfigureAwait(true);
			}
			public static async Task<string> PostJson( Uri url, string payload, int timeout, CancellationToken token ) => await TryPost(Post, url, payload, MediaTypeNames.Application.Json, timeout, token).ConfigureAwait(true);


			public static async Task<string> Post( Uri uri, string payload, int timeout, string contentType, CancellationToken token )
			{
				NetworkManager.ThrowIfNotConnected();

				var req = WebRequest.Create(uri); //req.Proxy = new WebProxy(ProxyString, true);
				req.Timeout = timeout;
				req.Method = "POST";
				req.ContentType = contentType;
				req.Headers[HttpRequestHeader.ContentEncoding] = Encoding.UTF8.ToString();

				Memory<byte> bytes = Encoding.UTF8.GetBytes(payload).AsMemory();
				req.ContentLength = bytes.Length;
				await using ( Stream os = await req.GetRequestStreamAsync().ConfigureAwait(true) )
				{
					await os.WriteAsync(bytes, token).ConfigureAwait(true); //Push it out there
				}

				using WebResponse resp = await req.GetResponseAsync(token).ConfigureAwait(true);

				await using Stream? stream = resp.GetResponseStream();
				using var sr = new StreamReader(stream ?? throw new Exception());
				string result = await sr.ReadToEndAsync().ConfigureAwait(true);

				return result.Trim();
			}


			//internal static async Task<string> TryPost( Func<Uri, string, CancellationToken, Task<string>> func, Uri url, string payload, CancellationToken token )
			//{
			//	try { return await func(url, payload, token).ConfigureAwait(true); }
			//	catch ( WebException we )
			//	{
			//		//WebExceptionStatus status = we.Status;
			//		Exception e = we.Status switch
			//					  {
			//						  //WebExceptionStatus.CacheEntryNotFound => new ,
			//						  WebExceptionStatus.ConnectFailure => new ConnectFailureException(we.Message, we, token),
			//						  WebExceptionStatus.ConnectionClosed => new ConnectionClosedException(we.Message, we, token),
			//						  WebExceptionStatus.KeepAliveFailure => new KeepAliveFailureException(we.Message, we, token),
			//						  //WebExceptionStatus.MessageLengthLimitExceeded => new ,
			//						  WebExceptionStatus.NameResolutionFailure => new NameResolutionException(we.Message, we, token),
			//						  //WebExceptionStatus.Pending => new ,
			//						  //WebExceptionStatus.PipelineFailure => new ,
			//						  //WebExceptionStatus.ProtocolError => new ,
			//						  //WebExceptionStatus.ProxyNameResolutionFailure => new ,
			//						  WebExceptionStatus.ReceiveFailure => new ReceiveFailureException(we.Message, we, token),
			//						  WebExceptionStatus.RequestCanceled => new RequestAbortedException(we.Message, we, token),
			//						  //WebExceptionStatus.RequestProhibitedByCachePolicy => new ,
			//						  //WebExceptionStatus.RequestProhibitedByProxy => new ,
			//						  //WebExceptionStatus.SecureChannelFailure => new ,
			//						  WebExceptionStatus.SendFailure => new SendFailureException(we.Message, we, token),
			//						  //WebExceptionStatus.ServerProtocolViolation => new ,
			//						  WebExceptionStatus.Success => null,
			//						  WebExceptionStatus.Timeout => new TimeoutException(we.Message, we),
			//						  //WebExceptionStatus.TrustFailure => new ,
			//						  //WebExceptionStatus.UnknownError => new ,
			//						  _ => null
			//					  };

			//		if ( e != null )
			//			throw e;

			//		throw;
			//	}
			//}
			//internal static async Task<string> TryPost( Func<Uri, string, int, CancellationToken, Task<string>> func, Uri url, string payload, int timeout, CancellationToken token )
			//{
			//	try { return await func(url, payload, timeout, token).ConfigureAwait(true); }
			//	catch ( WebException we )
			//	{
			//		//WebExceptionStatus status = we.Status;
			//		Exception e = we.Status switch
			//					  {
			//						  //WebExceptionStatus.CacheEntryNotFound => new ,
			//						  WebExceptionStatus.ConnectFailure => new ConnectFailureException(we.Message, we, token),
			//						  WebExceptionStatus.ConnectionClosed => new ConnectionClosedException(we.Message, we, token),
			//						  WebExceptionStatus.KeepAliveFailure => new KeepAliveFailureException(we.Message, we, token),
			//						  //WebExceptionStatus.MessageLengthLimitExceeded => new ,
			//						  WebExceptionStatus.NameResolutionFailure => new NameResolutionException(we.Message, we, token),
			//						  //WebExceptionStatus.Pending => new ,
			//						  //WebExceptionStatus.PipelineFailure => new ,
			//						  //WebExceptionStatus.ProtocolError => new ,
			//						  //WebExceptionStatus.ProxyNameResolutionFailure => new ,
			//						  WebExceptionStatus.ReceiveFailure => new ReceiveFailureException(we.Message, we, token),
			//						  WebExceptionStatus.RequestCanceled => new RequestAbortedException(we.Message, we, token),
			//						  //WebExceptionStatus.RequestProhibitedByCachePolicy => new ,
			//						  //WebExceptionStatus.RequestProhibitedByProxy => new ,
			//						  //WebExceptionStatus.SecureChannelFailure => new ,
			//						  WebExceptionStatus.SendFailure => new SendFailureException(we.Message, we, token),
			//						  //WebExceptionStatus.ServerProtocolViolation => new ,
			//						  WebExceptionStatus.Success => null,
			//						  WebExceptionStatus.Timeout => new TimeoutException(we.Message, we),
			//						  //WebExceptionStatus.TrustFailure => new ,
			//						  //WebExceptionStatus.UnknownError => new ,
			//						  _ => null
			//					  };

			//		if ( e != null )
			//			throw e;

			//		throw;
			//	}
			//}
			internal static async Task<string> TryPost( Func<Uri, string, int, string, CancellationToken, Task<string>> func, Uri url, string payload, string contentType, int timeout, CancellationToken token )
			{
				try { return await func(url, payload, timeout, contentType, token).ConfigureAwait(true); }
				catch ( WebException we )
				{
					Exception? e = we.Status switch
								   {
									   //WebExceptionStatus.CacheEntryNotFound => new ,
									   WebExceptionStatus.ConnectFailure => new ConnectFailureException(we.Message, we, token),
									   WebExceptionStatus.ConnectionClosed => new ConnectionClosedException(we.Message, we, token),
									   WebExceptionStatus.KeepAliveFailure => new KeepAliveFailureException(we.Message, we, token),
									   //WebExceptionStatus.MessageLengthLimitExceeded => new ,
									   WebExceptionStatus.NameResolutionFailure => new NameResolutionException(we.Message, we, token),
									   //WebExceptionStatus.Pending => new ,
									   //WebExceptionStatus.PipelineFailure => new ,
									   //WebExceptionStatus.ProtocolError => new ,
									   //WebExceptionStatus.ProxyNameResolutionFailure => new ,
									   WebExceptionStatus.ReceiveFailure => new ReceiveFailureException(we.Message, we, token),
									   WebExceptionStatus.RequestCanceled => new RequestAbortedException(we.Message, we, token),
									   //WebExceptionStatus.RequestProhibitedByCachePolicy => new ,
									   //WebExceptionStatus.RequestProhibitedByProxy => new ,
									   //WebExceptionStatus.SecureChannelFailure => new ,
									   WebExceptionStatus.SendFailure => new SendFailureException(we.Message, we, token),
									   //WebExceptionStatus.ServerProtocolViolation => new ,
									   WebExceptionStatus.Success => null,
									   WebExceptionStatus.Timeout => new TimeoutException(we.Message, we),
									   //WebExceptionStatus.TrustFailure => new ,
									   //WebExceptionStatus.UnknownError => new ,
									   _ => null
								   };

					if ( e != null )
						throw e;

					throw;
				}
			}
		}
	}
}