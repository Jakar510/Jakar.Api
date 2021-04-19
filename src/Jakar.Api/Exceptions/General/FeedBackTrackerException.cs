using System;
using System.Collections.Generic;
using Jakar.Api.Extensions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Exceptions.General
{
	public class FeedBackTrackerException : Exception
	{
		//var e = new FeedBackTrackerException(JsonConvert.SerializeObject(dict));
		public FeedBackTrackerException() { }
		public FeedBackTrackerException( string message ) : base(message) { }
		public FeedBackTrackerException( string message, Exception inner ) : base(message, inner) { }

		public FeedBackTrackerException( object dict ) : this(dict.ToPrettyJson()) { }
		public FeedBackTrackerException( object dict, Exception inner ) : this(dict.ToPrettyJson(), inner) { }
	}
}
