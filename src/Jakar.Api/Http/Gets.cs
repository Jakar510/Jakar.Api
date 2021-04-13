using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Enumerations;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class Gets
	{
		public static async Task<WebResponse> Get( this Uri url, int timeout, CancellationToken token, HeaderCollection? headers = null )
		{
			// https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx

			HttpWebRequest req = WebRequest.CreateHttp(url);
			req.Timeout = timeout;
			req.Method = "GET";
			
			headers ??= new HeaderCollection(MimeType.UrlEncodedContent);

			req.SetHeaders(headers);

			return await req.GetResponseAsync(token).ConfigureAwait(true);
		}

		public static async Task<string> TryGet( this Uri url, CancellationToken token, HeaderCollection? headers = null ) =>
			await url.TryGet(Requests.DEFAULT_TIMEOUT, token, headers).ConfigureAwait(true);

		public static async Task<string> TryGet( this Uri url, int timeout, CancellationToken token, HeaderCollection? headers = null ) =>
			await url.TryGet(WebResponseExtensions.AsString, timeout, token, headers).ConfigureAwait(true);

		public static async Task<TResult> TryGet<TResult>( this Uri url,
														   Func<WebResponse, Task<TResult>> handler,
														   int timeout,
														   CancellationToken token,
														   HeaderCollection? headers = null
		)
		{
			try
			{
				WebResponse reply = await url.Get(timeout, token, headers).ConfigureAwait(true);
				return await handler(reply).ConfigureAwait(true);
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
