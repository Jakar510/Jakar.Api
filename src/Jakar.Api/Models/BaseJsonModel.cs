using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	[Serializable]
	public class BaseJsonModel
	{
		[JsonExtensionData] public IDictionary<string, JToken>? AdditionalData { get; set; }
	}
}
