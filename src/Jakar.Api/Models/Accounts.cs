using System;
using System.Collections.Generic;
using Jakar.Api.Extensions;
using Newtonsoft.Json;

#pragma warning disable 1591

namespace Jakar.Api.Models
{
	[Serializable]
	public class Accounts
	{
		public CurrentUser Active { get; } = new();
		public Dictionary<long, User> All { get; set; } = new();


		public static Accounts FromString( string json ) => string.IsNullOrWhiteSpace(json) ? new Accounts() : json.FromJson<Accounts>();
		public string ToJson() => JsonConvert.SerializeObject(this);
	}
}