using System;
using System.Collections;
using System.Collections.Generic;
using Jakar.Api.Extensions;
using Jakar.Api.Interfaces;
using Newtonsoft.Json;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Models.Users
{
	[Serializable]
	public class Accounts<TUser, TActiveUser> : IEnumerable<TUser> where TUser : class, IUser
																   where TActiveUser : class, ICurrentUser<TUser>, new()
	{
		public TActiveUser Active { get; } = new();


		public List<TUser> All { get; set; } = new();

		public IEnumerator<TUser> GetEnumerator() => All.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
