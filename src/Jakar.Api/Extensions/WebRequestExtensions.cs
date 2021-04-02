using System;
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
	}
}