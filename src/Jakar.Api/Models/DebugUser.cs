using System;
using Newtonsoft.Json;

namespace Jakar.Api.Models
{
	[Serializable]
	public class DebugUser : User
	{
		[JsonProperty(nameof(CanDebug), Required = Required.Always)]
		public bool CanDebug { get; set; }

		public DebugUser() { }
	}
}