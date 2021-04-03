using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Jakar.Api.Extensions
{
	internal static class Converters
	{
		public static string Remove( this string source, string old ) => source.Replace(old, "", StringComparison.Ordinal);
		public static string Replace( this string source, string old, string newString ) => source.Replace(old, newString, StringComparison.Ordinal);
		public static bool Check( this string source, string search ) => source.Contains(search, StringComparison.Ordinal);


		//public TResult StringToEnum<TResult>(string input) where TResult : Enum
		//{
		//	if ( Enum.TryParse(input, true, out TResult type) )
		//	{
		//		return type;
		//	}

		//	return default;
		//}


		public static byte[] FromBase64String( this string s ) => Convert.FromBase64String(s);
		public static TResult JsonFromBase64String<TResult>( this string s ) => s.JsonFromBase64String<TResult>(Encoding.UTF8);

		public static TResult JsonFromBase64String<TResult>( this string s, Encoding encoding )
		{
			byte[] bytes = s.FromBase64String();
			string temp = encoding.GetString(bytes);
			return temp.FromJson<TResult>();
		}


		public static string ToPrettyJson( this object jsonSerializablePayload )
		{
			if ( jsonSerializablePayload is null ) throw new ArgumentNullException(nameof(jsonSerializablePayload));

			string json = jsonSerializablePayload.ToJson();
			return json.ToPrettyJson();
		}

		public static string ToPrettyJson( this string? json )
		{
			if ( string.IsNullOrWhiteSpace(json) ) throw new ArgumentNullException(nameof(json));

			JToken parsedJson = JToken.Parse(json);
			return parsedJson.ToString(Formatting.Indented);
		}


		public static string ToJson( this object s ) => JsonConvert.SerializeObject(s);
		public static TResult FromJson<TResult>( this string s ) => JsonConvert.DeserializeObject<TResult>(s) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));


		internal static string ToBase64( this object jsonSerializablePayload )
		{
			string temp = JsonConvert.SerializeObject(jsonSerializablePayload);
			return temp.ToBase64();
		}

		internal static string ToBase64( this string data ) => data.ToBase64(Encoding.UTF8);

		internal static string ToBase64( this string data, Encoding encoding )
		{
			byte[] payload = encoding.GetBytes(data);
			return Convert.ToBase64String(payload);
		}


		internal static MemoryStream ToStream( this string s )
		{
			byte[] buffer = FromBase64String(s);
			return new MemoryStream(buffer);
		}
	}
}
