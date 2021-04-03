using System;
using System.Collections.Generic;
using System.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public static class Versions
	{
		[Serializable]
		public class Version
		{
			public uint Major { get; private set; }
			public uint Minor { get; private set; }
			public uint Maintenance { get; private set; }
			public uint? Build { get; private set; }


			public Version( uint major, uint minor, uint maintenance ) => Set(major, minor, maintenance, null);
			public Version( uint major, uint minor, uint maintenance, uint? build ) => Set(major, minor, maintenance, build);

			public Version( IReadOnlyList<uint> items )
			{
				if ( items is null )
					throw new ArgumentNullException(nameof(items));

				if ( items.Count < 3 || items.Count > 4 )
					throw new ArgumentException(@"value doesn't contain the correct amount of items.", nameof(items));

				Set(items[0],
					items[1],
					items[2],
					items.Count == 3
						? (uint?) null
						: items[3]);
			}


			public static Version? TryParse( string? value ) => string.IsNullOrWhiteSpace(value) || !value.Contains('.', StringComparison.OrdinalIgnoreCase)
																	? null
																	: Parse(value);

			public static Version Parse( string? value )
			{
				if ( string.IsNullOrWhiteSpace(value) ) throw new ArgumentNullException(nameof(value));

				if ( !value.Contains('.', StringComparison.OrdinalIgnoreCase) ) throw new FormatException($"value \"{value}\" doesn't contain any periods.");

				List<uint> items = value.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList();

				return new Version(items);
			}


			private void Set( uint major, uint minor, uint maintenance, uint? build )
			{
				Major = major;
				Minor = minor;
				Maintenance = maintenance;
				Build = build;
			}

			public bool Compare( Version value )
			{
				if ( value is null )
					throw new ArgumentNullException(nameof(value));

				if ( Build is null ) // value.Build is null ||
					return value.Major >= Major && value.Minor >= Minor && value.Maintenance >= Maintenance;

				return value.Major >= Major && value.Minor >= Minor && value.Maintenance >= Maintenance && value.Build >= Build;
			}

			public override string ToString() => Build is null
													 ? $"{Major}.{Minor}.{Maintenance}"
													 : $"{Major}.{Minor}.{Maintenance}.{Build}";
		}



		public static Version AppVersion { get; } = Version.Parse(DeviceInfo.FullVersion);
	}
}
