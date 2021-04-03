﻿// unset

using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class WiFiException : Exception
	{
		public WiFiException() { }
		public WiFiException( string message ) : base(message) { }
		public WiFiException( string message, Exception inner ) : base(message, inner) { }
	}
}
