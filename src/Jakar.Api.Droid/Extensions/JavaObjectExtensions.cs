// unset

using System;
using Android.Runtime;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Droid.Extensions
{
	public static class JavaObjectExtensions
	{
		public static bool IsDisposed( this Java.Lang.Object obj ) => obj.Handle == IntPtr.Zero;
		public static bool IsAlive( this    Java.Lang.Object obj ) => !obj.IsDisposed();
		public static bool IsDisposed( this IJavaObject      obj ) => obj.Handle == IntPtr.Zero;
		public static bool IsAlive( this    IJavaObject      obj ) => !obj.IsDisposed();
	}
}
