using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class DictionaryExtensions
	{
		public static void ForEach<TKey, TValue>( this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action )
		{
			foreach ( ( TKey key, TValue value ) in dict ) { action(key, value); }
		}

		public static async Task ForEach<TKey, TValue>( this IDictionary<TKey, TValue> dict, Func<TKey, TValue, Task> action )
		{
			foreach ( ( TKey key, TValue value ) in dict ) { await action(key, value).ConfigureAwait(true); }
		}


		public static void AddDefault<TKey, TValue>( this IDictionary<TKey, TValue?> dict, TKey key ) { dict.Add(key, default); }

		public static void AddDefault<TKey, TValue>( this IDictionary<TKey, TValue?> dict, IEnumerable<TKey> keys )
		{
			foreach ( TKey value in keys ) { dict.AddDefault(value); }
		}
	}
}
