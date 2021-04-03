// unset

using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models.Users
{
	[Serializable]
	public class PhoneNumber
	{
		public long IsPrimary { get; set; }
		public string? Number { get; set; }
	}
}
