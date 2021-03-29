using System;
using Xamarin.Essentials;

namespace Jakar.Api
{
	internal class BarometerReader
	{
		public static BarometerReader Current => _Service.Value;
		private static Lazy<BarometerReader> _Service { get; } = new(Create, false);
		private static BarometerReader Create() => new();


		// Set speed delay for monitoring changes.
		public SensorSpeed Speed { get; protected set; } = SensorSpeed.Default;
		public string? Text { get; protected set; }
		public double Value { get; protected set; }
		public BarometerData Data { get; protected set; }

		public BarometerReader() => Barometer.ReadingChanged += Barometer_ReadingChanged;
		~BarometerReader() => Barometer.ReadingChanged -= Barometer_ReadingChanged;

		private void Barometer_ReadingChanged( object sender, BarometerChangedEventArgs e )
		{
			Data = e.Reading;
			Text = Data.PressureInHectopascals.ToString(Language.Current.CultureInfo);
			Value = Data.PressureInHectopascals;
		}

		public bool StartBarometer() => StartBarometer(SensorSpeed.UI);
		public bool StartBarometer( SensorSpeed speed )
		{
			try
			{
				Speed = speed;
				Barometer.Start(speed);
				return true;
			}
			catch ( FeatureNotSupportedException fnsEx )
			{
				// Feature not supported on device
				Debug.Current.HandleException(fnsEx);
				return false;
			}
			catch ( Exception ex )
			{
				// Other error has occurred.
				Debug.Current.HandleException(ex);
				return false;
			}
		}
		public bool StopBarometer()
		{
			try
			{
				if ( Barometer.IsMonitoring )
					Barometer.Stop();
				return true;
			}
			catch ( FeatureNotSupportedException fnsEx )
			{
				// Feature not supported on device
				Debug.Current.HandleException(fnsEx);
				return false;
			}
			catch ( Exception ex )
			{
				// Other error has occurred.
				Debug.Current.HandleException(ex);
				return false;
			}
		}
	}
}