using System;

namespace Jakar.Api.Interfaces
{
	public interface IBaseUrl
	{
		Uri GetUri();
		string GetBaseString();
	}
}