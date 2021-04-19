using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Jakar.Api.Extensions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public static class WebResponseExtensions
	{
		public static async Task<Stream?> AsStream( this WebResponse resp ) => await Task.FromResult(resp.GetResponseStream());

		public static async Task<TResult> AsJson<TResult>( this WebResponse resp )
		{
			string reply = await resp.AsString();

			return reply.FromJson<TResult>();
		}

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
