// unset

using System;

namespace Jakar.Api.Models
{
	[Serializable]
	public class PhoneNumber
	{
		public long IsPrimary { get; set; }
		public string? Number { get; set; }
	}
}