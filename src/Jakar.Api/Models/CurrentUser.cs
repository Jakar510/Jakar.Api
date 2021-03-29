using System;

namespace Jakar.Api.Models
{
	[Serializable]
	public class CurrentUser
	{
		public User? User { get; set; }
	}
}