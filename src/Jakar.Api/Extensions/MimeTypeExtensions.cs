using System;
using Jakar.Api.Enumerations;
using Jakar.Api.Http;
using Jakar.Api.Interfaces;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class MimeTypeExtensions
	{
		public static string ToUriScheme( this MimeType mime )
		{
			// TODO: get more uri schemes
			// https://docs.microsoft.com/en-us/office/client-developer/office-uri-schemes
			return mime switch
				   {
					   MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime)),
					   MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime)),

					   MimeType.Doc  => "ms-word",
					   MimeType.Docx => "ms-word",

					   MimeType.Xls  => "ms-excel",
					   MimeType.Xlsx => "ms-excel",

					   MimeType.Ppt => "ms-powerpoint",
					   MimeType.Pptx => "ms-powerpoint",

					   _ => "file",

					   //_ => throw new ArgumentOutOfRangeException(nameof(mime), mime, null)
				   };
		}


		public static string ToFileName( this MimeType mime, IAppSettings settings ) => mime.ToFileName(settings.AppName ?? throw new NullReferenceException(nameof(IAppSettings.AppName)));

		public static string ToFileName( this MimeType mime, string fileName )
		{
			if ( string.IsNullOrWhiteSpace(fileName) ) { throw new NullReferenceException(nameof(mime)); }

			string extension = mime switch
							   {
								   MimeType.Text      => "text",
								   MimeType.PlainText => "text",
								   MimeType.Html      => "HTML",
								   MimeType.Xml       => "XML",
								   MimeType.RichText  => "rtf",
								   MimeType.Css       => "css",
								   MimeType.Csv       => "csv",
								   MimeType.Calendar  => "ics",

								   MimeType.UrlEncodedContent => "text",
								   MimeType.Soap              => "soap",
								   MimeType.Stream            => "stream",
								   MimeType.Binary            => "bin",
								   MimeType.Rtf               => "rtf",
								   MimeType.Pdf               => "pdf",
								   MimeType.Json              => "json",
								   MimeType.XmlApp            => "XML",
								   MimeType.Xul               => "XUL",
								   MimeType.JavaScript        => "js",
								   MimeType.Vbs               => "VBS",

								   MimeType.Zip      => "ZIP",
								   MimeType.SevenZip => "7z",
								   MimeType.Bzip     => "bz",
								   MimeType.Bzip2    => "bz2",
								   MimeType.Gzip     => "gz",
								   MimeType.Tar      => "tar.gz",

								   MimeType.Doc  => "DOC",
								   MimeType.Docx => "DOCX",
								   MimeType.Xls  => "XLS",
								   MimeType.Xlsx => "XLSX",
								   MimeType.Ppt  => "PPT",
								   MimeType.Pptx => "PPTX",

								   MimeType.ThreeGppAudio  => "3gpp",
								   MimeType.ThreeGpp2Audio => "3gpp2",
								   MimeType.Aac            => "AAC",
								   MimeType.MpegAudio      => "MPEG",
								   MimeType.Mp3            => "MP3",
								   MimeType.Weba           => "WEBA",
								   MimeType.Wav            => "WAVE",

								   MimeType.ThreeGppVideo  => "3gpp",
								   MimeType.ThreeGpp2Video => "3gpp2",
								   MimeType.Mp4            => "MP4",
								   MimeType.MpegVideo      => "MPEG",
								   MimeType.Mpeg4          => "MPEG4",
								   MimeType.Webm           => "WEBM",
								   MimeType.H264           => "H264",
								   MimeType.Avi            => "AVI",
								   MimeType.Mov            => "MOV",
								   MimeType.Mpg            => "MPG",
								   MimeType.Ogg            => "OGG",
								   MimeType.Mkv            => "MKV",

								   MimeType.Gif  => "GIF",
								   MimeType.Tiff => "TIFF",
								   MimeType.Png  => "PNG",
								   MimeType.Jpeg => "JPEG",
								   MimeType.Jpg  => "JPG",
								   MimeType.Bmp  => "BMP",
								   MimeType.Webp => "WEBP",
								   MimeType.Icon => "ICON",
								   MimeType.Svg  => "svg",

								   MimeType.TrueType => "ttf",
								   MimeType.OpenType => "otf",
								   MimeType.FormData => "fd",


								   MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime)),
								   MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime)),
								   _                => throw new ArgumentOutOfRangeException(nameof(mime)),
							   };

			return $"{fileName}.{extension.ToLower()}";
		}


		public static MimeType ToMimeType( this string mime )
		{
			return mime switch
				   {
					   null => throw new NullReferenceException(nameof(mime)),

					   MimeTypeNames.Text.PLAIN                  => MimeType.PlainText,
					   MimeTypeNames.Text.HTML                   => MimeType.Html,
					   MimeTypeNames.Text.XML                    => MimeType.Xml,
					   MimeTypeNames.Text.RICH_TEXT              => MimeType.RichText,
					   MimeTypeNames.Text.CASCADING_STYLE_SHEETS => MimeType.Css,
					   MimeTypeNames.Text.COMMA_SEPARATED_VALUES => MimeType.Csv,
					   MimeTypeNames.Text.CALENDAR               => MimeType.Calendar,

					   MimeTypeNames.Application.URL_ENCODED_CONTENT => MimeType.UrlEncodedContent,
					   MimeTypeNames.Application.SOAP                => MimeType.Soap,
					   MimeTypeNames.Application.BINARY              => MimeType.Stream,
					   MimeTypeNames.Application.RTF                 => MimeType.Rtf,
					   MimeTypeNames.Application.PDF                 => MimeType.Pdf,
					   MimeTypeNames.Application.JSON                => MimeType.Json,
					   MimeTypeNames.Application.XML                 => MimeType.XmlApp,
					   MimeTypeNames.Application.XUL                 => MimeType.Xul,
					   MimeTypeNames.Application.JAVA_SCRIPT         => MimeType.JavaScript,
					   MimeTypeNames.Application.VBS                 => MimeType.Vbs,

					   MimeTypeNames.Application.Archive.ZIP       => MimeType.Zip,
					   MimeTypeNames.Application.Archive.SEVEN_ZIP => MimeType.SevenZip,
					   MimeTypeNames.Application.Archive.B_ZIP     => MimeType.Bzip,
					   MimeTypeNames.Application.Archive.B_ZIP_2   => MimeType.Bzip2,
					   MimeTypeNames.Application.Archive.GZIP      => MimeType.Gzip,
					   MimeTypeNames.Application.Archive.TAR       => MimeType.Tar,

					   MimeTypeNames.Application.MsOffice.DOC  => MimeType.Doc,
					   MimeTypeNames.Application.MsOffice.DOCX => MimeType.Docx,
					   MimeTypeNames.Application.MsOffice.XLS  => MimeType.Xls,
					   MimeTypeNames.Application.MsOffice.XLSX => MimeType.Xlsx,
					   MimeTypeNames.Application.MsOffice.PPT  => MimeType.Ppt,
					   MimeTypeNames.Application.MsOffice.PPTX => MimeType.Pptx,

					   MimeTypeNames.Audio.THREE_GPP_AUDIO  => MimeType.ThreeGppAudio,
					   MimeTypeNames.Audio.THREE_GPP2_AUDIO => MimeType.ThreeGpp2Audio,
					   MimeTypeNames.Audio.AAC              => MimeType.Aac,
					   MimeTypeNames.Audio.MPEG             => MimeType.MpegAudio,
					   MimeTypeNames.Audio.MP3              => MimeType.Mp3,
					   MimeTypeNames.Audio.WEBA             => MimeType.Weba,
					   MimeTypeNames.Audio.WAVE             => MimeType.Wav,

					   MimeTypeNames.Video.THREE_GPP_VIDEO  => MimeType.ThreeGppVideo,
					   MimeTypeNames.Video.THREE_GPP2_VIDEO => MimeType.ThreeGpp2Video,
					   MimeTypeNames.Video.MP4              => MimeType.Mp4,
					   MimeTypeNames.Video.MPEG             => MimeType.MpegVideo,
					   MimeTypeNames.Video.MPEG4            => MimeType.Mpeg4,
					   MimeTypeNames.Video.WEBM             => MimeType.Webm,
					   MimeTypeNames.Video.H264             => MimeType.H264,
					   MimeTypeNames.Video.AVI              => MimeType.Avi,
					   MimeTypeNames.Video.MOV              => MimeType.Mov,
					   MimeTypeNames.Video.MPG              => MimeType.Mpg,
					   MimeTypeNames.Video.OGG              => MimeType.Ogg,
					   MimeTypeNames.Video.MKV              => MimeType.Mkv,

					   MimeTypeNames.Image.GIF  => MimeType.Gif,
					   MimeTypeNames.Image.TIFF => MimeType.Tiff,
					   MimeTypeNames.Image.PNG  => MimeType.Png,
					   MimeTypeNames.Image.JPEG => MimeType.Jpeg,
					   MimeTypeNames.Image.JPG  => MimeType.Jpg,
					   MimeTypeNames.Image.BMP  => MimeType.Bmp,
					   MimeTypeNames.Image.WEBP => MimeType.Webp,
					   MimeTypeNames.Image.ICON => MimeType.Icon,
					   MimeTypeNames.Image.SVG  => MimeType.Svg,

					   MimeTypeNames.Font.TRUE_TYPE      => MimeType.TrueType,
					   MimeTypeNames.Font.OPEN_TYPE      => MimeType.OpenType,
					   MimeTypeNames.MultiPart.FORM_DATA => MimeType.FormData,

					   _ => MimeType.Unknown
				   };
		}


		public static string ToContentType( this MimeType mime )
		{
			return mime switch
				   {
					   MimeType.Text      => MimeTypeNames.Text.PLAIN,
					   MimeType.PlainText => MimeTypeNames.Text.PLAIN,
					   MimeType.Html      => MimeTypeNames.Text.HTML,
					   MimeType.Xml       => MimeTypeNames.Text.XML,
					   MimeType.RichText  => MimeTypeNames.Text.RICH_TEXT,
					   MimeType.Css       => MimeTypeNames.Text.CASCADING_STYLE_SHEETS,
					   MimeType.Csv       => MimeTypeNames.Text.COMMA_SEPARATED_VALUES,
					   MimeType.Calendar  => MimeTypeNames.Text.CALENDAR,

					   MimeType.UrlEncodedContent => MimeTypeNames.Application.URL_ENCODED_CONTENT,
					   MimeType.Soap              => MimeTypeNames.Application.SOAP,
					   MimeType.Stream            => MimeTypeNames.Application.BINARY,
					   MimeType.Binary            => MimeTypeNames.Application.BINARY,
					   MimeType.Rtf               => MimeTypeNames.Application.RTF,
					   MimeType.Pdf               => MimeTypeNames.Application.PDF,
					   MimeType.Json              => MimeTypeNames.Application.JSON,
					   MimeType.XmlApp            => MimeTypeNames.Application.XML,
					   MimeType.Xul               => MimeTypeNames.Application.XUL,
					   MimeType.JavaScript        => MimeTypeNames.Application.JAVA_SCRIPT,
					   MimeType.Vbs               => MimeTypeNames.Application.VBS,

					   MimeType.Zip      => MimeTypeNames.Application.Archive.ZIP,
					   MimeType.SevenZip => MimeTypeNames.Application.Archive.SEVEN_ZIP,
					   MimeType.Bzip     => MimeTypeNames.Application.Archive.B_ZIP,
					   MimeType.Bzip2    => MimeTypeNames.Application.Archive.B_ZIP_2,
					   MimeType.Gzip     => MimeTypeNames.Application.Archive.GZIP,
					   MimeType.Tar      => MimeTypeNames.Application.Archive.TAR,

					   MimeType.Doc  => MimeTypeNames.Application.MsOffice.DOC,
					   MimeType.Docx => MimeTypeNames.Application.MsOffice.DOCX,
					   MimeType.Xls  => MimeTypeNames.Application.MsOffice.XLS,
					   MimeType.Xlsx => MimeTypeNames.Application.MsOffice.XLSX,
					   MimeType.Ppt  => MimeTypeNames.Application.MsOffice.PPT,
					   MimeType.Pptx => MimeTypeNames.Application.MsOffice.PPTX,

					   MimeType.ThreeGppAudio  => MimeTypeNames.Audio.THREE_GPP_AUDIO,
					   MimeType.ThreeGpp2Audio => MimeTypeNames.Audio.THREE_GPP2_AUDIO,
					   MimeType.Aac            => MimeTypeNames.Audio.AAC,
					   MimeType.MpegAudio      => MimeTypeNames.Audio.MPEG,
					   MimeType.Mp3            => MimeTypeNames.Audio.MP3,
					   MimeType.Weba           => MimeTypeNames.Audio.WEBA,
					   MimeType.Wav            => MimeTypeNames.Audio.WAVE,

					   MimeType.ThreeGppVideo  => MimeTypeNames.Video.THREE_GPP_VIDEO,
					   MimeType.ThreeGpp2Video => MimeTypeNames.Video.THREE_GPP2_VIDEO,
					   MimeType.Mp4            => MimeTypeNames.Video.MP4,
					   MimeType.MpegVideo      => MimeTypeNames.Video.MPEG,
					   MimeType.Mpeg4          => MimeTypeNames.Video.MPEG4,
					   MimeType.Webm           => MimeTypeNames.Video.WEBM,
					   MimeType.H264           => MimeTypeNames.Video.H264,
					   MimeType.Avi            => MimeTypeNames.Video.AVI,
					   MimeType.Mov            => MimeTypeNames.Video.MOV,
					   MimeType.Mpg            => MimeTypeNames.Video.MPG,
					   MimeType.Ogg            => MimeTypeNames.Video.OGG,
					   MimeType.Mkv            => MimeTypeNames.Video.MKV,

					   MimeType.Gif  => MimeTypeNames.Image.GIF,
					   MimeType.Tiff => MimeTypeNames.Image.TIFF,
					   MimeType.Png  => MimeTypeNames.Image.PNG,
					   MimeType.Jpeg => MimeTypeNames.Image.JPEG,
					   MimeType.Jpg  => MimeTypeNames.Image.JPG,
					   MimeType.Bmp  => MimeTypeNames.Image.BMP,
					   MimeType.Webp => MimeTypeNames.Image.WEBP,
					   MimeType.Icon => MimeTypeNames.Image.ICON,
					   MimeType.Svg  => MimeTypeNames.Image.SVG,

					   MimeType.TrueType => MimeTypeNames.Font.TRUE_TYPE,
					   MimeType.OpenType => MimeTypeNames.Font.OPEN_TYPE,
					   MimeType.FormData => MimeTypeNames.MultiPart.FORM_DATA,

					   MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime), mime, null),
					   MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime), mime, null),
					   _                => throw new ArgumentOutOfRangeException(nameof(mime), mime, null)
				   };
		}
	}
}
