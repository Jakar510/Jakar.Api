using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class EnumerableExtensions
	{
		public static bool IsEmpty( this ICollection collection ) => collection.Count == 0;
		public static bool IsEmpty<TItem>( this ICollection<TItem> collection ) => collection.Count == 0;


		public static void Add<TItem>( this IList<TItem> list, IEnumerable<TItem> items ) => items.ForEach(list.Add);
		public static void Add<TItem>( this IList<TItem> list, params TItem[] items ) => items.ForEach(list.Add);


		public static void Remove<TItem>( this IList<TItem> list, IEnumerable<TItem> items ) => items.ForEach(item => list.Remove(item));
		public static void Remove<TItem>( this IList<TItem> list, params TItem[] items ) => items.ForEach(item => list.Remove(item));


		public static async Task ForEachAsync<TItem>( this IEnumerable<TItem> list, Func<TItem, Task> action, bool continueOnCapturedContext = true )
		{
			foreach ( TItem item in list ) { await action(item).ConfigureAwait(continueOnCapturedContext); }
		}

		
		public static IEnumerable<(int index, TItem item)> Enumerate<TItem>( this IEnumerable<TItem> enumerable )
		{
			var index = 0;

			foreach ( TItem item in enumerable )
			{
				yield return ( index, item );
				index++;
			}
		}
	}
}
