using System;
using System.Collections.Generic;


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
	}
}
