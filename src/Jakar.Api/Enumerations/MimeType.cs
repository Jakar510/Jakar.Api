// unset

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Enumerations
{
	public enum MimeType
	{
		NotSet,
		Unknown,

		// Text
		Html,
		Text,
		Xml,

		// Office
		Doc,
		Docx,
		Xls,
		Xlsx,

		// Image
		Gif,
		Icon,
		Jpeg,
		Jpg,
		Png,
		Bmp,

		// Video
		Mov,
		Mp4,
		Avi,
		Mkv,
		Ogg,
		Wav,
		ThreeGp, // 3GP
		Mpeg,
		Mpg,

		// Doc
		Pdf,
		Rtf,
		Tif,

		// Script
		Vbs,
		Css,
		JavaScript,

		// Misc
		Zip,
		Stream,
	}
}
