using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;
using Jakar.Api.Statics;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class Posts
	{
		// https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx
		// https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads

		public static async Task<string> PostJson( this Uri url, string payload, CancellationToken token, HeaderCollection? headers = null ) =>

			// Activity.Current.Settings.OutgoingActivityText = payload;
			await PostJson(url,
						   payload,
						   Requests.DEFAULT_TIMEOUT,
						   token,
						   headers).ConfigureAwait(true);

		public static async Task<string> PostJson( this Uri url,
												   string payload,
												   int timeout,
												   CancellationToken token,
												   HeaderCollection? headers = null
		) => await url.TryPost(WebResponseExtensions.AsString,
							   Encoding.UTF8.GetBytes(payload).AsMemory(),
							   headers ?? new HeaderCollection(MimeType.Json),
							   timeout,
							   token).ConfigureAwait(true);


		public static async Task<WebResponse> Post( this Uri url,
													ReadOnlyMemory<byte> payload,
													HeaderCollection headers,
													int timeout,
													CancellationToken token
		)
		{
			NetworkManager.ThrowIfNotConnected();

			HttpWebRequest req = WebRequest.CreateHttp(url); //req.Proxy = new WebProxy(ProxyString, true);
			req.Timeout = timeout;
			req.Method = "POST";

			req.SetHeaders(headers);

			req.ContentLength = payload.Length;

			await using ( Stream os = await req.GetRequestStreamAsync().ConfigureAwait(true) )
			{
				await os.WriteAsync(payload, token).ConfigureAwait(true); //Push it out there
			}

			return await req.GetResponseAsync(token).ConfigureAwait(true);
		}


		public static async Task<WebResponse> Post( this Uri url,
													MultipartFormDataContent payload,
													int timeout,
													CancellationToken token
		)
		{
			NetworkManager.ThrowIfNotConnected();

			HttpWebRequest req = WebRequest.CreateHttp(url); //req.Proxy = new WebProxy(ProxyString, true);
			req.Timeout = timeout;
			req.Method = "POST";

			req.SetHeaders(payload);

			await using ( Stream os = await req.GetRequestStreamAsync().ConfigureAwait(true) )
			{
				await payload.CopyToAsync(os).ConfigureAwait(true); // Push it out there
			}


			return await req.GetResponseAsync(token).ConfigureAwait(true);
		}


		public static async Task<string> TryPost( this Uri url,
												  string payload,
												  MimeType contentType,
												  CancellationToken token
		) =>
			await url.TryPost(payload,
							  contentType.ToContentType(),
							  token).ConfigureAwait(true);


		public static async Task<string> TryPost( this Uri url,
												  string payload,
												  string contentType,
												  CancellationToken token
		) =>
			await url.TryPost(WebResponseExtensions.AsString,
							  payload,
							  contentType,
							  Requests.DEFAULT_TIMEOUT,
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															string payload,
															MimeType contentType,
															CancellationToken token
		) =>
			await url.TryPost(handler,
							  payload,
							  contentType.ToContentType(),
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															string payload,
															string contentType,
															CancellationToken token
		) =>
			await url.TryPost(handler,
							  payload,
							  contentType,
							  Requests.DEFAULT_TIMEOUT,
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															string payload,
															string contentType,
															int timeout,
															CancellationToken token
		)
		{
			var headers = new HeaderCollection(contentType);

			return await url.TryPost(handler,
									 Encoding.UTF8.GetBytes(payload).AsMemory(),
									 headers,
									 timeout,
									 token).ConfigureAwait(true);
		}


		public static async Task<TResult> TryPost<TResult, TPayload>( this Uri url,
																	  Func<WebResponse, Task<TResult>> handler,
																	  Func<TPayload, ReadOnlyMemory<byte>> serializer,
																	  TPayload payload,
																	  HeaderCollection headers,
																	  int timeout,
																	  CancellationToken token
		) =>
			await url.TryPost(handler,
							  serializer(payload),
							  headers,
							  timeout,
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															ReadOnlyMemory<byte> payload,
															HeaderCollection headers,
															CancellationToken token
		) =>
			await url.TryPost(handler,
							  payload,
							  headers,
							  Requests.DEFAULT_TIMEOUT,
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															ReadOnlyMemory<byte> payload,
															HeaderCollection headers,
															int timeout,
															CancellationToken token
		)
		{
			try
			{
				using WebResponse response = await url.Post(payload,
															headers,
															timeout,
															token).ConfigureAwait(true);

				return await handler(response).ConfigureAwait(true);
			}
			catch ( WebException we )
			{
				Exception? e = we.ConvertException(token);
				if ( e is not null ) { throw e; }

				throw;
			}
		}


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															MultipartFormDataContent payload,
															CancellationToken token
		) =>
			await url.TryPost(handler,
							  payload,
							  Requests.DEFAULT_TIMEOUT,
							  token).ConfigureAwait(true);


		public static async Task<TResult> TryPost<TResult>( this Uri url,
															Func<WebResponse, Task<TResult>> handler,
															MultipartFormDataContent payload,
															int timeout,
															CancellationToken token
		)
		{
			try
			{
				using WebResponse response = await url.Post(payload,
															timeout,
															token).ConfigureAwait(true);

				return await handler(response).ConfigureAwait(true);
			}
			catch ( WebException we )
			{
				Exception? e = we.ConvertException(token);
				if ( e is not null ) { throw e; }

				throw;
			}
		}
	}
}
