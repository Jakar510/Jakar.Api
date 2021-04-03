using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models.Users
{
	[Serializable]
	public class Email
	{
		public long IsPrimary { get; set; }
		public string? Address { get; set; }
	}
}
