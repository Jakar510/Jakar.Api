using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using LocalAuthentication;
using TrueLogicMobile.API;
using UIKit;

namespace TrueLogicMobile.iOS.Services
{
	public class LocalAuthHelper : Api.Biometrics.IAuthHelper
	{
		public string GetLocalAuthLabelText()
		{
			Api.Biometrics.LocalAuthType localAuthType = GetLocalAuthType();
			return localAuthType.ToString();
		}
		public string GetLocalAuthIcon()
		{
			Api.Biometrics.LocalAuthType localAuthType = GetLocalAuthType();
			return localAuthType switch
				   {
					   Api.Biometrics.LocalAuthType.PassCode => "LockIcon",
					   Api.Biometrics.LocalAuthType.TouchId => "TouchIdIcon",
					   Api.Biometrics.LocalAuthType.FaceId => "FaceIdIcon",
					   _ => string.Empty
				   };
		}
		public string GetLocalAuthUnlockText()
		{
			Api.Biometrics.LocalAuthType localAuthType = GetLocalAuthType();
			return localAuthType switch
				   {
					   Api.Biometrics.LocalAuthType.PassCode => "UnlockWithPasscode",
					   Api.Biometrics.LocalAuthType.TouchId => "UnlockWithTouchID",
					   Api.Biometrics.LocalAuthType.FaceId => "UnlockWithFaceID",
					   _ => string.Empty
				   };
		}
		public bool IsLocalAuthAvailable => GetLocalAuthType() != Api.Biometrics.LocalAuthType.None;
		public void Authenticate( Action onSuccess, Action onFailure )
		{
			using var context = new LAContext();
			if ( context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError AuthError) || context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out AuthError) )
			{
				var replyHandler = new LAContextReplyHandler((success, error) =>
				{
					if (success)
					{
						onSuccess?.Invoke();
					}
					else
					{
						onFailure?.Invoke();
					}
				});

				context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, "Please Authenticate To Proceed", replyHandler);
			}

			AuthError?.Dispose();
		}
		public Api.Biometrics.LocalAuthType GetLocalAuthType()
		{
			using var localAuthContext = new LAContext();
			if ( !localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out NSError _) ) return Api.Biometrics.LocalAuthType.None;
			if ( !localAuthContext.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError _) ) return Api.Biometrics.LocalAuthType.PassCode;
			if ( GetOsMajorVersion() >= 11 && localAuthContext.BiometryType == LABiometryType.FaceId )
			{
				return Api.Biometrics.LocalAuthType.FaceId;
			}
			return Api.Biometrics.LocalAuthType.TouchId;
		}
		public int GetOsMajorVersion() => int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0], Api.Language.Current.CultureInfo);
	}
}