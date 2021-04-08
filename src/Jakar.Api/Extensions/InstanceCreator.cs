﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class InstanceCreator
	{
		public static TItem Create<TItem>( params object[] args ) => (TItem) Activator.CreateInstance(typeof(TItem), args);
		public static NullReferenceException CreateException( params Type[] args ) => new($"constructor not found. Requested arg types: {args}");
	}



	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class InstanceCreator<T1, T2, T3, TInstance>
	{
		public static Func<T1, T2, T3, TInstance> Create { get; } = CreateInstance();

		private static Func<T1, T2, T3, TInstance> CreateInstance()
		{
			Type[] argsTypes =
			{
				typeof(T1),
				typeof(T2),
				typeof(T3)
			};

			ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
			if ( constructor is null ) throw InstanceCreator.CreateException(argsTypes);

			ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();

			// ReSharper disable once CoVariantArrayConversion
			return Expression.Lambda<Func<T1, T2, T3, TInstance>>(Expression.New(constructor, args), args).Compile();
		}
	}



	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class InstanceCreator<T1, T2, TInstance>
	{
		public static Func<T1, T2, TInstance> Create { get; } = CreateInstance();

		private static Func<T1, T2, TInstance> CreateInstance()
		{
			Type[] argsTypes =
			{
				typeof(T1),
				typeof(T2)
			};

			ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
			if ( constructor is null ) throw InstanceCreator.CreateException(argsTypes);

			ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();

			// ReSharper disable once CoVariantArrayConversion
			return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(constructor, args), args).Compile();
		}
	}



	[Xamarin.Forms.Internals.Preserve(true, false)]
	public static class InstanceCreator<T1, TInstance>
	{
		public static Func<T1, TInstance> Create { get; } = CreateInstance();

		private static Func<T1, TInstance> CreateInstance()
		{
			Type[] argsTypes =
			{
				typeof(T1)
			};

			ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
			if ( constructor is null ) throw InstanceCreator.CreateException(argsTypes);

			ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();

			// ReSharper disable once CoVariantArrayConversion
			return Expression.Lambda<Func<T1, TInstance>>(Expression.New(constructor, args), args).Compile();
		}
	}
}
