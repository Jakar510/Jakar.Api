﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Jakar.Api.Exceptions.Networking;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	// public interface IHeaderCollection
	// {
	// 	public IDictionary<HttpRequestHeader, object>? Headers { get; }
	// 	public IDictionary<string, object>? StringHeaders { get; }
	// }



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
					// WebException is thrown when request.Abort() is called, but there may be many other reasons, propagate the WebException to the caller correctly
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
		
		public static Exception? Handle( this WebException we, CancellationToken token )
		{
			return we.Status switch
				   {
					   //WebExceptionStatus.CacheEntryNotFound => new ,
					   WebExceptionStatus.ConnectFailure   => new ConnectFailureException(we.Message, we, token),
					   WebExceptionStatus.ConnectionClosed => new ConnectionClosedException(we.Message, we, token),
					   WebExceptionStatus.KeepAliveFailure => new KeepAliveFailureException(we.Message, we, token),

					   //WebExceptionStatus.MessageLengthLimitExceeded => new ,
					   WebExceptionStatus.NameResolutionFailure => new NameResolutionException(we.Message, we, token),

					   //WebExceptionStatus.Pending => new ,
					   //WebExceptionStatus.PipelineFailure => new ,
					   //WebExceptionStatus.ProtocolError => new ,
					   //WebExceptionStatus.ProxyNameResolutionFailure => new ,
					   WebExceptionStatus.ReceiveFailure  => new ReceiveFailureException(we.Message, we, token),
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
		}
		
		// // public static void SetHeaders( this WebRequest request, IHeaderCollection headers ) { }
		// public static void SetHeaders( this WebRequest request, IDictionary<HttpRequestHeader, object> headers ) { headers.ForEach(request.SetHeader); }
		// public static void SetHeaders( this WebRequest request, IDictionary<string, object> headers ) { headers.ForEach(request.SetHeader); }
		//
		// public static void SetHeader( this WebRequest request, HttpRequestHeader key, object value )
		// {
		// 	// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
		// 	switch ( key )
		// 	{
		// 		// case HttpRequestHeader.From:
		// 		// 	break;
		// 		//
		// 		// case HttpRequestHeader.Host:
		// 		// 	break;
		//
		// 		// case HttpRequestHeader.Allow:
		// 		// 	break;
		//
		// 		// case HttpRequestHeader.Authorization:
		// 		// 	request.AuthenticationLevel = value.ToString();
		// 		// 	request.Credentials = value.ToString();
		// 		// 	break;
		//
		// 		case HttpRequestHeader.UserAgent:
		// 			throw new NotSupportedException($"{typeof(WebRequest).FullName} does not support UserAgent assignment.");
		//
		// 		case HttpRequestHeader.ContentLength:
		// 			if ( value is long contentLength ) { request.ContentLength = contentLength; }
		// 			else { throw new HeaderException(key, value.GetType(), typeof(long)); }
		//
		// 			break;
		//
		// 		case HttpRequestHeader.ContentType:
		// 			request.SetContentType(value);
		// 			break;
		//
		// 		case HttpRequestHeader.ProxyAuthorization:
		// 			if ( value is IWebProxy proxy ) { request.Proxy = proxy; }
		// 			else { throw new HeaderException(key, value.GetType(), typeof(IWebProxy)); }
		//
		// 			break;
		//
		// 		default:
		// 			request.Headers[key] = value.ToString();
		// 			return;
		// 	}
		// }
		//
		// public static void SetHeader( this WebRequest request, string key, object value )
		// {
		// 	if ( key == "Content-Type" ) { request.SetContentType(value); }
		// 	else { request.Headers.Add(key, value.ToJson()); }
		// }
		//
		// public static void SetHeaders( this WebRequest request, MultipartFormDataContent data ) => request.SetHeaders(data.Headers);
		//
		// public static void SetHeaders( this WebRequest request, HttpContentHeaders headers )
		// {
		// 	foreach ( ( string key, IEnumerable<string> items ) in headers )
		// 	{
		// 		string value = items.ToJson();
		//
		// 		if ( Enum.TryParse(key, true, out HttpRequestHeader result) ) { request.SetHeader(result, value); }
		// 		else { request.SetHeader(key, value); }
		// 	}
		// }


		public static void SetContentType( this WebRequest request, object value )
		{
			request.ContentType = value switch
								  {
									  IEnumerable<string> items => items.First(),
									  string item               => item,
									  _                         => throw new HeaderException(HttpRequestHeader.ContentType, value.GetType(), typeof(string))
								  };
		}

		public static void SetHeaders( this HttpWebRequest request, MultipartFormDataContent data ) => request.SetHeaders(data.Headers);

		public static void SetHeaders( this HttpWebRequest request, HttpContentHeaders headers )
		{
			foreach ( ( string key, IEnumerable<string> items ) in headers )
			{
				if ( Enum.TryParse(key, true, out HttpRequestHeader httpRequestHeader) ) { request.SetHeader(httpRequestHeader, items); }
				else { request.SetHeader(key, items); }
			}
		}


		public static void SetHeaders( this HttpWebRequest request, IDictionary<HttpRequestHeader, object> headers ) { headers.ForEach(request.SetHeader); }
		public static void SetHeaders( this HttpWebRequest request, IDictionary<string, object> headers ) { headers.ForEach(request.SetHeader); }

		public static void SetHeader( this HttpWebRequest request, HttpRequestHeader key, object value )
		{
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch ( key )
			{
				// case HttpRequestHeader.From:
				// 	break;
				//
				// case HttpRequestHeader.Host:
				// 	break;

				// case HttpRequestHeader.Allow:
				// 	break;

				// case HttpRequestHeader.Authorization:
				// 	request.AuthenticationLevel = value.ToString();
				// 	request.Credentials = value.ToString();
				// 	break;

				case HttpRequestHeader.Accept:
					request.Accept = value.ToString();
					break;

				case HttpRequestHeader.Connection:
					request.Connection = value.ToString();
					break;

				case HttpRequestHeader.ContentLength:
					if ( value is long contentLength ) { request.ContentLength = contentLength; }
					else { throw new HeaderException(key, value.GetType(), typeof(long)); }

					break;

				case HttpRequestHeader.ContentType:
					request.SetContentType(value);
					break;

				case HttpRequestHeader.Cookie:
					if ( value is Cookie cookie ) { request.CookieContainer.Add(cookie); }
					else { throw new HeaderException(key, value.GetType(), typeof(Cookie)); }

					break;

				case HttpRequestHeader.Date:
					if ( value is DateTime dateTime ) { request.Date = dateTime; }
					else { throw new HeaderException(key, value.GetType(), typeof(DateTime)); }

					break;

				case HttpRequestHeader.Expect:
					request.Expect = value.ToString();
					break;

				case HttpRequestHeader.KeepAlive:
					if ( value is bool keepAlive ) { request.KeepAlive = keepAlive; }
					else { throw new HeaderException(key, value.GetType(), typeof(bool)); }

					break;

				case HttpRequestHeader.MaxForwards:
					if ( value is int redirects ) { request.MaximumAutomaticRedirections = redirects; }
					else { throw new HeaderException(key, value.GetType(), typeof(int)); }

					break;

				case HttpRequestHeader.ProxyAuthorization:
					if ( value is IWebProxy proxy ) { request.Proxy = proxy; }
					else { throw new HeaderException(key, value.GetType(), typeof(IWebProxy)); }

					break;

				case HttpRequestHeader.TransferEncoding:
					request.TransferEncoding = value.ToString();
					break;

				case HttpRequestHeader.UserAgent:
					request.UserAgent = value.ToString();
					break;

				default:
					request.Headers[key] = value.ToString();
					return;
			}
		}

		public static void SetHeader( this HttpWebRequest request, string key, object value )
		{
			if ( key == "Content-Type" ) { request.SetContentType(value); }
			else { request.Headers.Add(key, value.ToJson()); }
		}


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
