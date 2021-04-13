using System.Collections.Generic;
using System.Net;
using Jakar.Api.Enumerations;
using Jakar.Api.Extensions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public class HeaderCollection : Dictionary<string, object>
	{
		public HeaderCollection() { }
		public HeaderCollection( MimeType contentType ) : this(contentType.ToContentType()) { }
		public HeaderCollection( string contentType ) => Add(HttpRequestHeader.ContentType, contentType);


		public void Add( HttpRequestHeader header, object value ) => Add(header.ToString(), value);

		public void Add( IDictionary<string, object> headers ) { headers.ForEach(Add); }

		public void Add( IDictionary<HttpRequestHeader, object> headers ) { headers.ForEach(Add); }
	}
}
