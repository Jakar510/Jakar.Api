using System;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Interfaces
{
	public interface IBaseUrl
	{
		Uri    GetUri();
		string GetBaseString();
	}
}
