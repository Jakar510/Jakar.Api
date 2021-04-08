using System;
using System.Threading;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class Pings
	{
		public static async Task<bool> TryPing( this Uri url, string payload, int timeout, CancellationToken token )
		{
			try
			{
				string reply = await url.PostJson(payload, timeout, token).ConfigureAwait(true);

				return !string.IsNullOrWhiteSpace(reply);
			}
			catch ( TimeoutException ) { return false; }
		}

		public static async Task<string> Ping( this Uri url, CancellationToken token ) => await url.Ping(Requests.SHORT_TIMEOUT, token).ConfigureAwait(true);
		public static async Task<string> Ping( this Uri url, int timeout, CancellationToken token ) => await url.TryGet(timeout, token).ConfigureAwait(true);
	}
}
