using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Extensions
{
	public static class ShellExtensions
	{
		public static async Task GoToAsync( this Shell shell, bool root, params Type[] types ) => await shell.GoToAsync(GetPath(root, types)).ConfigureAwait(true);

		public static async Task GoToAsync( this Shell shell, Type type, bool root, IDictionary<string, object> parameters ) =>
			await shell.GoToAsync(type.Name, root, parameters).ConfigureAwait(true);

		public static async Task GoToAsync( this Shell shell, string type, bool root, IDictionary<string, object> parameters ) =>
			await shell.GoToAsync(type.GetPath(parameters, root)).ConfigureAwait(true);

		public static string GetPath( bool root = false, params Type[] types ) => types.Parameterize(root);

		public static string GetPath( this Type type, IDictionary<string, object>? parameters = null, bool root = false ) => type.Name.GetPath(parameters, root);

		public static string GetPath( this string type, IDictionary<string, object>? parameters = null, bool root = false )
		{
			if ( parameters is null ) return type;

			string result = type + parameters.Parameterize();

			return root
					   ? $"//{result}"
					   : result;
		}

		public static string Parameterize( this Type[] types, bool root ) => types.Aggregate(root
																								 ? $"//"
																								 : "",
																							 Parameterize);

		private static string Parameterize( string previous, Type type ) { return previous + $"/{type.Name}"; }


		public static string Parameterize( this IDictionary<string, object> parameters ) => parameters.Aggregate("?", Parameterize);

		private static string Parameterize( string previous, KeyValuePair<string, object> pair )
		{
			( string? key, object? value ) = pair;

			return previous + $"{key}={value},";
		}
	}
}
