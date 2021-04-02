// unset

using Xamarin.Forms.Internals;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	[Preserve(true, false)]
	public static class NumberExtensions
	{
		public static float ToFloat( this double value ) => (float) value;
		public static int ToInt( this double value ) => (int) value;
		
	}
}