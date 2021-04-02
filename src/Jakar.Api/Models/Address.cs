using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	[Serializable]
	public class Address
	{
		public long IsPrimary { get; set; }

		public string? LineOne { get; init; }
		public string? LineTwo { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public long Zip { get; set; }
		public string? Country { get; set; }
	}
}