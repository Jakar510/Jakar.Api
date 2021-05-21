using System;
using System.Threading.Tasks;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.Statics
{
	public class Biometrics
	{
		public interface IAuthHelperForIOS
		{
			public bool IsLocalAuthAvailable { get; }

			public string GetLocalAuthUnlockText();
			public string GetLocalAuthIcon();
			public string GetLocalAuthLabelText();
			public void   Authenticate( Action?     onSuccess, Action?     onFailure );
			public void   Authenticate( Func<Task>? onSuccess, Func<Task>? onFailure );
			public int    GetOsMajorVersion();

			public LocalAuthType GetLocalAuthType();
		}



		public enum LocalAuthType
		{
			None,
			PassCode,
			TouchId,
			FaceId
		}
	}
}
