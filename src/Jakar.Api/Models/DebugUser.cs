using System;
using Newtonsoft.Json;

#pragma warning disable 1591

#nullable enable
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