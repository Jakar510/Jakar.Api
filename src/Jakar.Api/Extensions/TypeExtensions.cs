using System;
using System.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class TypeExtensions
	{
		public static bool IsEqual( this Type value, Type other )
		{
			// _Accessory.Checked = _RadioCell.Value.GetType().IsValueType
			// 						 ? Equals(_RadioCell.Value, _SelectedValue)
			// 						 : ReferenceEquals(_RadioCell.Value, _SelectedValue);
			if ( value is null ) throw new NullReferenceException(nameof(value));
			if ( other is null ) throw new NullReferenceException(nameof(other));

			if ( value.IsValueType ) { return value == other; }

			if ( value.IsClass ) { return value == other; }

			if ( value.IsEnum ) { return value == other; }

			return ReferenceEquals(value, other);
		}

		public static bool IsOneOf( this Type value, params Type[] items ) => items.Any(value.IsEqual);


		public static bool IsOneOf<TValue>( this TValue value, params Type[] items ) where TValue : class => items.Any(value.IsEqual);
		public static bool IsEqual<TValue>( this TValue value, Type other ) where TValue : class => value.GetType() == other;


		public static bool IsBuiltInType( this Type type )
		{
			return type.FullName switch
				   {
					   "System.Boolean" => true,
					   "System.Byte"    => true,
					   "System.SByte"   => true,
					   "System.Char"    => true,
					   "System.Int16"   => true,
					   "System.UInt16"  => true,
					   "System.Int32"   => true,
					   "System.UInt32"  => true,
					   "System.Int64"   => true,
					   "System.UInt64"  => true,
					   "System.Single"  => true,
					   "System.Double"  => true,
					   "System.Decimal" => true,
					   "System.String"  => true,
					   "System.Object"  => true,
					   _                => false
				   };
		}
	}
}
