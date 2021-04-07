#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Interfaces
{
	public interface IUser
	{
		public long Id { get; set; }
		public bool IsActive { get; set; }

		public string? UserName { get; }

		public string? FullName { get; }
	}
}
