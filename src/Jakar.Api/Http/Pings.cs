using System;
using System.Threading;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class Pings
	{
		public static async Task<string> Ping( this Uri url, CancellationToken token, HeaderCollection? headers = null ) => await url.Ping(Requests.SHORT_TIMEOUT, token, headers).ConfigureAwait(true);
		public static async Task<string> Ping( this Uri url, int timeout, CancellationToken token, HeaderCollection? headers = null ) => await url.TryGet(timeout, token, headers).ConfigureAwait(true);

		public static async Task<bool> Ping( this Uri url, string payload, int timeout, CancellationToken token, HeaderCollection? headers = null )
		{
			try
			{
				string reply = await url.PostJson(payload, timeout, token, headers).ConfigureAwait(true);

				return !string.IsNullOrWhiteSpace(reply);
			}
			catch ( TimeoutException ) { return false; }
		}

	}
}
