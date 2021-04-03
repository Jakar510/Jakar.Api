using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jakar.Api.Models;
using Jakar.Api.Models.Users;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class AccountManager
	{
		public static AccountManager Current => _Service.Value;
		private static Lazy<AccountManager> _Service { get; } = new(Create, false);
		private static AccountManager Create() => new();


		private Accounts _Accounts { get; set; } = new();
		public AccountManager() { Task.Run(Load); }

		private async Task Load()
		{
			await using var file = new FileData(FileSystem.AccountsFileName);
			Accounts? items = await file.ReadFromFileAsync<Accounts>().ConfigureAwait(true);
			_Accounts = items ?? new Accounts();
		}

		private async Task Save()
		{
			await using var file = new FileData(FileSystem.AccountsFileName);
			string json = _Accounts.ToJson();
			await file.WriteToFileAsync(json).ConfigureAwait(true);
		}

		public async Task Logout()
		{
			_Accounts.Active.User = null;
			await Save().ConfigureAwait(true);
		}


		public User? GetAccount() => _Accounts.Active.User;

		public User? GetAccount( long id ) => _Accounts.All.TryGetValue(id, out User? user)
												  ? user
												  : null;

		public User? GetAccount( string userName ) => GetAccount(s => s.UserName == userName);

		public User? GetAccount( Func<User, bool> check )
		{
			Dictionary<long, User>.ValueCollection users = _Accounts.All.Values;

			return users.FirstOrDefault(check);
		}

		public User? GetAccount( User user )
		{
			Dictionary<long, User>.ValueCollection users = _Accounts.All.Values;

			return users.FirstOrDefault(item => item == user);
		}


		public long? GetAccountID( User user ) => _Accounts.All.FirstOrDefault(item => item.Value == user).Key;


		public long AddAccount( User user ) => AddAccount(_Accounts.All.Count, user);

		public long AddAccount( long id, User user )
		{
			if ( GetAccount(user) != null )
				return GetAccountID(user) ?? throw new NullReferenceException(nameof(GetAccountID));

			_Accounts.All.Add(id, user);

			return id;
		}
	}
}
