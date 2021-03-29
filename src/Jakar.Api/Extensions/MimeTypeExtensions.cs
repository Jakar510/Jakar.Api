using System;
using Jakar.Api.Enumerations;

namespace Jakar.Api.Extensions
{
	public static class MimeTypeExtensions
	{
		public static string ToUriScheme( this MimeType mime )
		{
			// https://docs.microsoft.com/en-us/office/client-developer/office-uri-schemes
			return mime switch
				   {
					   MimeType.Doc => "ms-word",
					   MimeType.Docx => "ms-word",
					   MimeType.Xls => "ms-excel",
					   MimeType.Xlsx => "ms-excel",
					   MimeType.NotSet => throw new ArgumentOutOfRangeException(nameof(mime)),
					   MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime)),
					   //    MimeType.Html => expr,
					   //    MimeType.Text => expr,
					   //    MimeType.Xml => expr,
					   //    MimeType.Gif => expr,
					   //    MimeType.Icon => expr,
					   //    MimeType.Jpeg => expr,
					   //    MimeType.Jpg => expr,
					   //    MimeType.Png => expr,
					   //    MimeType.Bmp => expr,
					   //    MimeType.Mov => expr,
					   //    MimeType.Mp4 => expr,
					   //    MimeType.Avi => expr,
					   //    MimeType.Mkv => expr,
					   //    MimeType.Ogg => expr,
					   //    MimeType.Wav => expr,
					   //    MimeType.ThreeGp => expr,
					   //    MimeType.Mpeg => expr,
					   //    MimeType.Mpg => expr,
					   //    MimeType.Pdf => expr,
					   //    MimeType.Rtf => expr,
					   //    MimeType.Tif => expr,
					   //    MimeType.Vbs => expr,
					   //    MimeType.Css => expr,
					   //    MimeType.JavaScript => expr,
					   //    MimeType.Zip => expr,
					   //    MimeType.Stream => expr,
					   _ => "file",
					   //_ => throw new ArgumentOutOfRangeException(nameof(mime), mime, null)
				   };
		}
		public static string ToFileName( this MimeType mime ) => mime.ToFileName(AppSettings.Current.AppName ?? throw new NullReferenceException(nameof(AppSettings.Current.AppName)));
		public static string ToFileName( this MimeType mime, string fileName )
		{
			string extension = mime switch
							   {
								   MimeType.JavaScript => "js",
								   MimeType.Css => "css",
								   MimeType.Vbs => "vbs",

								   MimeType.Doc => "doc",
								   MimeType.Docx => "docx",
								   MimeType.Xls => "xls",
								   MimeType.Xlsx => "xlsx",

								   MimeType.Icon => "icon",
								   MimeType.Gif => "gif",
								   MimeType.Png => "png",
								   MimeType.Jpeg => "jpeg",
								   MimeType.Jpg => "jpg",

								   MimeType.Mov => "mov",
								   MimeType.Mp4 => "mp4",
								   MimeType.Avi => "avi",
								   MimeType.Mkv => "mkv",
								   MimeType.Ogg => "ogg",
								   MimeType.Wav => "wav",
								   MimeType.ThreeGp => "3gp",
								   MimeType.Mpeg => "mpeg",
								   MimeType.Mpg => "mpg",
								   MimeType.Tif => "tif",
								   MimeType.Bmp => "bmp",


								   MimeType.Xml => "xml",
								   MimeType.Text => "txt",
								   MimeType.Html => "html",
								   MimeType.Rtf => "rtf",

								   MimeType.Pdf => "pdf",

								   MimeType.Zip => "zip",
								   MimeType.Stream => "data",

								   MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime)),
								   MimeType.NotSet => throw new ArgumentOutOfRangeException(nameof(mime)),
								   _ => throw new ArgumentOutOfRangeException(nameof(mime)),
							   };

			return $"{fileName}.{extension}";
		}
	}
}