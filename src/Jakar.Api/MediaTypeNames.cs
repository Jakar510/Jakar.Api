// unset

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class MediaTypeNames
	{
		public static class Text
		{
			public const string PLAIN = "text/plain";
			public const string HTML = "text/html";
			public const string XML = "text/xml";
			public const string RICH_TEXT = "text/richtext";
		}



		public static class Application
		{
			public const string URL_ENCODED_CONTENT = "application/x-www-form-urlencoded";
			public const string SOAP = "application/soap+xml";
			public const string OCTET = "application/octet-stream";
			public const string RTF = "application/rtf";
			public const string PDF = "application/pdf";
			public const string ZIP = "application/zip";
			public const string JSON = "application/json";
			public const string XML = "application/xml";
		}



		public static class Image
		{
			public const string GIF = "image/gif";
			public const string TIFF = "image/tiff";
			public const string JPEG = "image/jpeg";
		}



		public static class MultiPart
		{
			public const string FORM_DATA = "multipart/form-data";
		}
	}
}
