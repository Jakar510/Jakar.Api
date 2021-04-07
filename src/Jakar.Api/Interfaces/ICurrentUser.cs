#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Interfaces
{
	public interface ICurrentUser<TUser> where TUser : IUser
	{
		public TUser? User { get; set; }
	}
}
