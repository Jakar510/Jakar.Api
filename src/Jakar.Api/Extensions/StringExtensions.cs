using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class StringExtensions
	{
		public static byte[] ToByteArray( this string s ) => s.ToByteArray(Encoding.UTF8);
		public static byte[] ToByteArray( this string s, Encoding encoding ) => encoding.GetBytes(s);

		public static Memory<byte> ToMemory( this string s ) => s.ToMemory(Encoding.UTF8);
		public static Memory<byte> ToMemory( this string s, Encoding encoding ) => s.ToByteArray(encoding).AsMemory();

		public static ReadOnlyMemory<byte> ToReadOnlyMemory( this string s ) => s.ToReadOnlyMemory(Encoding.UTF8);
		public static ReadOnlyMemory<byte> ToReadOnlyMemory( this string s, Encoding encoding ) => s.ToMemory(encoding);


		public static string ConvertToString( this byte[] s ) => s.ConvertToString(Encoding.UTF8);
		public static string ConvertToString( this byte[] s, Encoding encoding ) => encoding.GetString(s);

		public static string ConvertToString( this Memory<byte> s ) => s.ConvertToString(Encoding.UTF8);
		public static string ConvertToString( this Memory<byte> s, Encoding encoding ) => s.ToArray().ConvertToString(encoding);

		public static string ConvertToString( this ReadOnlyMemory<byte> s ) => s.ConvertToString(Encoding.UTF8);
		public static string ConvertToString( this ReadOnlyMemory<byte> s, Encoding encoding ) => s.ToArray().ConvertToString(encoding);


		public static string RemoveAll( this string source, string old ) => source.Replace(old, "", StringComparison.Ordinal);
		public static string RemoveAll( this string source, char old ) => source.Replace(new string(old, 1), "");

		public static string ReplaceAll( this string source, string old, string newString ) => source.Replace(old, newString, StringComparison.Ordinal);
		public static string ReplaceAll( this string source, char old, char newString ) => source.Replace(old, newString);
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

		

		public static string ToJson( this JToken json ) => json.ToJson(Formatting.Indented);
		public static string ToJson( this JToken json, Formatting formatting ) => json.ToString(formatting);

		public static string ToJson( this string json ) => json.FromJson().ToJson();
		public static string ToJson( this string json, JsonLoadSettings settings ) => json.FromJson(settings).ToJson();
		public static string ToJson( this string json, Formatting formatting ) => json.FromJson().ToJson(formatting);
		public static string ToJson( this string json, JsonLoadSettings settings, Formatting formatting ) => json.FromJson(settings).ToJson(formatting);
		
		public static string ToPrettyJson( this object s ) => s.ToJson(Formatting.Indented);
		public static string ToJson( this object s ) => JsonConvert.SerializeObject(s);
		public static string ToJson( this object s, Formatting formatting ) => JsonConvert.SerializeObject(s, formatting);
		public static string ToJson( this object s, JsonSerializerSettings settings ) => s.ToJson(Formatting.Indented, settings);
		public static string ToJson( this object s, Formatting formatting, JsonSerializerSettings settings ) => JsonConvert.SerializeObject(s, formatting, settings);
		public static string ToJson( this object s, params JsonConverter[] converters ) => s.ToJson(Formatting.Indented, converters);
		public static string ToJson( this object s, Formatting formatting, params JsonConverter[] converters ) => JsonConvert.SerializeObject(s, formatting, converters);


		public static TResult FromJson<TResult>( this string s ) => JsonConvert.DeserializeObject<TResult>(s) ?? throw new NullReferenceException(nameof(JsonConvert.DeserializeObject));
		public static JObject FromJson( this string s ) => JObject.Parse(s) ?? throw new NullReferenceException(nameof(JObject.Parse));
		public static JObject FromJson( this string s, JsonLoadSettings settings ) => JObject.Parse(s, settings) ?? throw new NullReferenceException(nameof(JObject.Parse));


		public static string ToBase64( this object jsonSerializablePayload )
		{
			string temp = jsonSerializablePayload.ToJson();
			return temp.ToBase64();
		}

		public static string ToBase64( this string data ) => data.ToBase64(Encoding.UTF8);

		public static string ToBase64( this string data, Encoding encoding )
		{
			byte[] payload = encoding.GetBytes(data);
			return Convert.ToBase64String(payload);
		}


		public static MemoryStream ToStream( this string s )
		{
			byte[] buffer = s.FromBase64String();
			return new MemoryStream(buffer);
		}
	}
}
