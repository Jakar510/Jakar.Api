using System;

namespace Jakar.Api.Models
{
	[Serializable]
	public class Email
	{
		public long IsPrimary { get; set; }
		public string? Address { get; set; }
	}
}