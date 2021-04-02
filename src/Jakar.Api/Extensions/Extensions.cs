using System;
using System.Linq;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class Extensions
	{
		public static bool IsOneOf<TValue>( this TValue value, params TValue[] items ) => items.Any(item => value.IsEqual(item));
		public static bool IsEqual<TValue>( this TValue value, TValue other ) => ( value ?? throw new NullReferenceException(nameof(value)) ).Equals(other);
		public static bool IsOneOfClass<TValue>( this TValue value, params TValue[] items ) where TValue : class => items.Any(value.IsEqual);


		// public static bool IsOneOfType<TValue>( this TValue value, params object[] items ) where TValue : class => IsOneOfType(value, items.Select(x => x.GetType()).ToArray());
		public static bool IsOneOf<TValue>( this TValue value, params Type[] items ) where TValue : class => items.Any(value.IsEqual);
		public static bool IsEqual<TValue>( this TValue value, Type other ) where TValue : class => value.GetType() == other;

	}
}