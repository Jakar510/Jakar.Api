using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Permissions = Jakar.Api.Statics.Permissions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public class LocationManager
	{
		public enum State
		{
			Default,
			Success,
			UnknownError,
			PermissionIssue,
			FeatureNotEnabled,
			FeatureNotSupported,
			IsFromMockProvider,
		}



		public State Status { get; protected set; }
		public Location? Location { get; protected set; }

		protected void Reset()
		{
			Status = default;
			Location = null;
		}

		protected async Task<State> GetLocationAsync( GeolocationAccuracy accuracy = GeolocationAccuracy.Default )
		{
			Reset();
			var request = new GeolocationRequest(accuracy);
			Location = await Geolocation.GetLocationAsync(request).ConfigureAwait(true);

			if ( Location is null ) { return State.UnknownError; }

			Status = Location.IsFromMockProvider
						 ? State.IsFromMockProvider
						 : State.Success;

			// try
			// {
			// }
			// catch ( FeatureNotSupportedException ) // Handle not supported on device exception
			// {
			// 	Status = StatusState.FeatureNotSupported;
			// }
			// catch ( FeatureNotEnabledException ) // Handle not enabled on device exception
			// {
			// 	Status = StatusState.FeatureNotEnabled;
			// }
			// catch ( PermissionException ) // Handle permission exception
			// {
			// 	Status = StatusState.PermissionIssue;
			// }
			// catch ( Exception ) // Unable to get location
			// {
			// 	Status = StatusState.UnknownError;
			// }

			return Status;
		}

		public async Task<bool> Update()
		{
			if ( await Permissions.LocationWhenInUsePermission().ConfigureAwait(true) != PermissionStatus.Granted ) { return false; }

			State status = await GetLocationAsync().ConfigureAwait(true);
			return status == State.Success;
		}


		public static async Task<Location?> GetLocation()
		{
			var manager = new LocationManager();

			if ( await manager.Update() ) { return manager.Location; }

			return null;
		}
	}
}
