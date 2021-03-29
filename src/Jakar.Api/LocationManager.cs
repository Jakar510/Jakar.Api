using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Jakar.Api
{
	internal class LocationManager
	{
		internal static LocationManager Current { get; } = new();

		public StatusState Status { get; protected set; }
		public Location? Location { get; protected set; }

		protected void Reset()
		{
			Status = default;
			Location = null;
		}
		protected async Task<StatusState> GetLocationAsync()
		{
			try
			{
				Reset();
				var request = new GeolocationRequest(GeolocationAccuracy.Best);
				Location = await Geolocation.GetLocationAsync(request).ConfigureAwait(true);

				if ( Location is null )
					return StatusState.UnknownError;

				Status = Location.IsFromMockProvider ? StatusState.IsFromMockProvider : StatusState.Success;
			}
			catch ( FeatureNotSupportedException ) // Handle not supported on device exception
			{
				Status = StatusState.FeatureNotSupported;
			}
			catch ( FeatureNotEnabledException ) // Handle not enabled on device exception
			{
				Status = StatusState.FeatureNotEnabled;
			}
			catch ( PermissionException ) // Handle permission exception
			{
				Status = StatusState.PermissionIssue;
			}
			catch ( Exception ex ) // Unable to get location
			{
				await Debug.Current.HandleExceptionAsync(ex).ConfigureAwait(true);
				Status = StatusState.UnknownError;
			}

			return Status;
		}
		internal async Task<bool> Update()
		{
			if ( await Permissions.LocationWhenInUsePermission().ConfigureAwait(true) != PermissionStatus.Granted ) { return false; }

			StatusState status = await GetLocationAsync().ConfigureAwait(true);
			return status == StatusState.Success;
		}


		public enum StatusState
		{
			Default,
			Success,
			UnknownError,
			PermissionIssue,
			FeatureNotEnabled,
			FeatureNotSupported,
			IsFromMockProvider,
		}


		internal static async Task<Plugin.Media.Abstractions.Location?> GetLocation()
		{
			if ( !await Current.Update().ConfigureAwait(true) ) return null;

			Location? location = Current.Location;
			if ( location is null ) return null;

			return new Plugin.Media.Abstractions.Location()
				   {
					   Altitude = location.Altitude ?? double.NaN,
					   HorizontalAccuracy = location.Accuracy ?? double.NaN,
					   Latitude = location.Latitude,
					   Longitude = location.Longitude,
					   Speed = location.Speed ?? double.NaN,
					   Timestamp = new DateTime(location.Timestamp.Ticks),
				   };
		}
	}
}