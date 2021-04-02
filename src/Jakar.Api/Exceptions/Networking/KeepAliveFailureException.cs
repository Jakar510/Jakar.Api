﻿using System;
using System.Net;
using System.Threading;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.Networking
{
	/// <summary>
	/// The connection for a request that specifies the Keep-alive header was closed unexpectedly.
	/// </summary>
	public sealed class KeepAliveFailureException : WebException
	{
		public CancellationToken Token { get; }
		public KeepAliveFailureException() { }
		public KeepAliveFailureException( string message ) : base(message) { }
		public KeepAliveFailureException( string message, Exception inner ) : base(message, inner) { }
		public KeepAliveFailureException( string message, WebException source, CancellationToken token ) : this(message, source ?? throw new NullReferenceException(nameof(source)), source.Status, source.Response, token) { }
		public KeepAliveFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse response ) : base(message, inner, status, response) { }
		public KeepAliveFailureException( string message, Exception inner, WebExceptionStatus status, WebResponse response, CancellationToken token ) : base(message, inner, status, response)
		{
			Token = token;
			Data["token"] = Token.ToString();
		}
	}
}