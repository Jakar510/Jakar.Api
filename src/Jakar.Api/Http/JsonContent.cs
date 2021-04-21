using System.Net.Http;
using System.Text;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Http
{
	public class JsonContent : StringContent
	{
		public JsonContent( string content ) : this(content, Encoding.UTF8) { }
		public JsonContent( string content, Encoding encoding ) : base(content, encoding, MimeTypeNames.Application.JSON) { }
	}
}
