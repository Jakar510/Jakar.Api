using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class WebRequestExtensions
	{
		public static async Task<WebResponse> GetResponseAsync( this WebRequest request, CancellationToken token, bool useSynchronizationContext = true )
		{
			// https://stackoverflow.com/questions/19211972/getresponseasync-does-not-accept-cancellationtoken

			if ( request is null ) throw new ArgumentNullException(nameof(request));

			await using ( token.Register(request.Abort, useSynchronizationContext) )
			{
				try { return await request.GetResponseAsync().ConfigureAwait(true); }
				catch ( WebException ex )
				{
					// WebException is thrown when request.Abort() is called,
					// but there may be many other reasons,
					// propagate the WebException to the caller correctly
					if ( token.IsCancellationRequested )
					{
						// the WebException will be available as Exception.InnerException
						throw new OperationCanceledException(ex.Message, ex, token);
					}

					// cancellation hasn't been requested, rethrow the original WebException
					throw;
				}
			}
		}

		public static void AddHeader( this WebRequest request, IDictionary<HttpRequestHeader, object> headers ) { headers.ForEach(request.AddHeader); }

		public static void AddHeader( this WebRequest request, HttpRequestHeader key, object value ) { request.Headers[key] = value.ToString(); }


		public static async Task<string> AsString( this WebResponse resp )
		{
			await using Stream? stream = resp.GetResponseStream();
			using var sr = new StreamReader(stream ?? throw new Exception());
			string result = await sr.ReadToEndAsync().ConfigureAwait(true);

			return result.Trim();
		}

		public static async Task<byte[]> AsBytes( this WebResponse resp )
		{
			await using Stream? stream = resp.GetResponseStream();
			if ( stream is null ) { throw new NullReferenceException(nameof(stream)); }

			await using var sr = new MemoryStream();
			await stream.CopyToAsync(sr).ConfigureAwait(true);

			return sr.ToArray();
		}

		public static async Task<ReadOnlyMemory<byte>> AsReadyOnlyBytes( this WebResponse resp )
		{
			byte[] bytes = await resp.AsBytes().ConfigureAwait(true);
			return bytes.AsMemory();
		}
	}
}
