using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Jakar.Api.Models;
using Jakar.Api.Models.Users;
using Jakar.Api.Statics;


#pragma warning disable 1591

namespace Jakar.Api
{
	public class AccountManager<TUser, TActiveUser> : ICollection<TUser> where TUser : class, IUser
																		 where TActiveUser : class, ICurrentUser<TUser>, new()
	{
		protected Accounts<TUser, TActiveUser> _Accounts { get; set; } = new();
		public TActiveUser CurrentUser => _Accounts.Active;


		public AccountManager() { Task.Run(Load); }

		protected virtual async Task Load()
		{
			await using var file = new FileData(FileSystem.AccountsFileName);
			Accounts<TUser, TActiveUser>? items = await file.ReadFromFileAsync<Accounts<TUser, TActiveUser>>().ConfigureAwait(true);
			_Accounts = items ?? new Accounts<TUser, TActiveUser>();
		}

		protected virtual async Task Save()
		{
			await using var file = new FileData(FileSystem.AccountsFileName);
			string json = _Accounts.ToJson();
			await file.WriteToFileAsync(json).ConfigureAwait(true);
		}

		public async Task Logout()
		{
			_Accounts.Active.User = default;
			await Save().ConfigureAwait(true);
		}


		public TUser? Get() => _Accounts.Active.User;
		public IEnumerable<TUser> Get( Func<TUser, bool> check ) => _Accounts.Where(check);
		public TUser? Get( long id ) => _Accounts.All.FirstOrDefault(item => item.Id == id);
		public TUser? Get( TUser user ) => _Accounts.All.FirstOrDefault(item => item.Id == user.Id);
		public TUser? Get( string userName ) => Get(s => s.UserName == userName).FirstOrDefault();


		public void Add( TUser user )
		{
			if ( Contains(user) ) { return; }

			_Accounts.All.Add(user);
		}

		public void Add( IEnumerable<TUser> users )
		{
			foreach ( var item in users ) { Add(item); }
		}

		public int Count => _Accounts.All.Count;
		public bool IsReadOnly => false;


		public IEnumerable<TUser> Active() => Get(item => item.IsActive);
		public IEnumerable<TUser> Disabled() => Get(item => !item.IsActive);


		public void Clear() => _Accounts.All.Clear();
		public bool Contains( TUser user ) => Get(user) is not null;
		public void CopyTo( TUser[] array, int arrayIndex ) { _Accounts.All.CopyTo(array, arrayIndex); }
		public bool Remove( TUser item ) => _Accounts.All.Remove(item);


		public IEnumerator<TUser> GetEnumerator() => _Accounts.All.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		// public int IndexOf( TUser item ) { return _Accounts.All.IndexOf(item); }
		// public void Insert( int index, TUser item ) { _Accounts.All.Insert(index, item); }
		// public void RemoveAt( int index ) { _Accounts.All.RemoveAt(index); }

		// public TUser this[ int index ]
		// {
		// 	get => _Accounts.All[index];
		// 	set => _Accounts.All[index] = value;
		// }
	}
}
