// unset

using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Enumerations
{
	public enum MimeType
	{
		NotSet,
		Unknown,
		
		// Text
		Text,
		PlainText,
		Html,
		Xml,
		RichText,
		Css,
		Csv,
		Calendar,

		// application
		UrlEncodedContent,
		Soap,
		Binary,
		Stream,
		Rtf,
		Pdf,
		Json,
		XmlApp,
		Xul,
		JavaScript,
		Vbs,

		// application.archive
		Zip,
		SevenZip,
		Bzip,
		Bzip2,
		Gzip,
		Tar,

		// application.office
		Doc,
		Docx,
		Xls,
		Xlsx,
		Ppt,
		Pptx,

		// audio
		ThreeGppAudio,
		ThreeGpp2Audio,
		Aac,
		MpegAudio,
		Mp3,
		Weba,
		Wav,

		// video
		ThreeGppVideo,
		ThreeGpp2Video,
		Mp4,
		MpegVideo,
		Mpeg4,
		Webm,
		H264,
		Avi,
		Mov,
		Mpg,
		Ogg,
		Mkv,

		// images
		Gif,
		Tiff,
		Png,
		Jpeg,
		Jpg,
		Bmp,
		Webp,
		Icon,
		Svg,

		// font
		TrueType,
		OpenType,

		// multipart
		FormData,
	}
}
