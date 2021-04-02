using System;
using System.Collections.Generic;

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

		public FeedBackTrackerException( Dictionary<string, string> dict ) : this(Debug.Current.PrettyJson(dict ?? throw new ArgumentNullException(nameof(dict)))) { }
		public FeedBackTrackerException( Dictionary<string, string> dict, Exception inner ) : this(Debug.Current.PrettyJson(dict ?? throw new ArgumentNullException(nameof(dict))), inner) { }
	}
}