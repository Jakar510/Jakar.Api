using System;
using Newtonsoft.Json;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters.Json
{
	public sealed class StringDoubleConverter : JsonConverter
	{
		public override bool CanWrite => false;
		public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer ) => throw new NotSupportedException();

		public override object ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer ) => double.TryParse(reader?.Value?.ToString() ?? "", out double n)
																																	   ? n
																																	   : double.NaN;

		public override bool CanConvert( Type objectType ) => objectType == typeof(string) || objectType == null;
	}
}
