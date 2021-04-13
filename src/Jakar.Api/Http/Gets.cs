﻿using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class Gets
	{
		public static async Task<WebResponse> Get( this Uri url, int timeout, CancellationToken token )
		{
			// https://www.hanselman.com/blog/HTTPPOSTsAndHTTPGETsWithWebClientAndCAndFakingAPostBack.aspx

			var req = WebRequest.Create(url);
			req.Timeout = timeout;
			req.Method = "GET";
			req.ContentType = MimeTypeNames.Application.URL_ENCODED_CONTENT;
			req.Headers[HttpRequestHeader.ContentEncoding] = Encoding.UTF8.ToString();

			return await req.GetResponseAsync(token).ConfigureAwait(true);
		}

		public static async Task<string> TryGet( this Uri url, CancellationToken token ) => await url.TryGet(Requests.DEFAULT_TIMEOUT, token).ConfigureAwait(true);
		public static async Task<string> TryGet( this Uri url, int timeout, CancellationToken token ) => await url.TryGet(WebResponseExtensions.AsString, timeout, token).ConfigureAwait(true);

		public static async Task<TResult> TryGet<TResult>( this Uri url,
														   Func<WebResponse, Task<TResult>> handler,
														   int timeout,
														   CancellationToken token
		)
		{
			try
			{
				WebResponse reply = await url.Get(timeout, token).ConfigureAwait(true);
				return await handler(reply).ConfigureAwait(true);
			}
			catch ( WebException we )
			{
				Exception? e = we.Handle(token);
				if ( e is not null ) { throw e; }

				throw;
			}
		}
	}
}
