// unset

using System;
using Newtonsoft.Json;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Converters.Json
{
	public class IntToEnumConverter<TEnum> : JsonConverter where TEnum : Enum
	{
		public override void WriteJson( JsonWriter writer, object? value, JsonSerializer serializer )
		{
			string result = value switch
							{
								int number => number.ToString(),
								TEnum n    => ( Enum.ToObject(typeof(TEnum), n) ).ToString(),
								_          => throw new JsonReaderException(nameof(value))
							};

			writer.WriteValue(result);
		}

		public override object ReadJson( JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer )
		{
			return existingValue switch
				   {
					   int value     => Enum.ToObject(typeof(TEnum), value),
					   TEnum screens => screens,
					   _             => throw new JsonReaderException(nameof(existingValue))
				   };
		}

		public override bool CanConvert( Type objectType ) => objectType == typeof(int) || objectType == typeof(TEnum);
	}
}
