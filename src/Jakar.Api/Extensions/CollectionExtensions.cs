using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms.Internals;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class CollectionExtensions
	{
		public static bool IsEmpty( this ICollection collection ) => collection.Count == 0;
		public static bool IsEmpty<TItem>( this ICollection<TItem> collection ) => collection.Count == 0;


		public static void Add<TItem>( this IList<TItem> list, IEnumerable<TItem> items ) => items.ForEach(list.Add);
		public static void Add<TItem>( this IList<TItem> list, params TItem[] items ) => items.ForEach(list.Add);

		public static void Remove<TItem>( this IList<TItem> list, IEnumerable<TItem> items ) => items.ForEach(item => list.Remove(item));
		public static void Remove<TItem>( this IList<TItem> list, params TItem[] items ) => items.ForEach(item => list.Remove(item));
	}
}
