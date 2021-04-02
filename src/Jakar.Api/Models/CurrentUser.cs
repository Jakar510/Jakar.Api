using System;

#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models
{
	[Serializable]
	public class CurrentUser
	{
		public User? User { get; set; }
	}
}