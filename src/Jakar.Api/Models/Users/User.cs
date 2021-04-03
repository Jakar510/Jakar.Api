using System;
using System.Collections.Generic;
using Newtonsoft.Json;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models.Users
{
	[Serializable]
	public class User
	{
		[JsonProperty(nameof(IsActive), Required = Required.Always)]
		public long Id { get; set; }

		[JsonProperty(nameof(IsActive), Required = Required.Always)]
		public bool IsActive { get; set; }

		public List<Address>? Address { get; set; }
		public List<PhoneNumber>? PhoneNumbers { get; set; }
		public List<Email>? Emails { get; set; }


		public string? UserName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public virtual string? FullName => $"{FirstName} {LastName}";

		public string? Type { get; set; }

		public User() { }
	}
}
