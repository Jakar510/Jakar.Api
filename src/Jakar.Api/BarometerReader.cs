using System;
using Xamarin.Essentials;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api
{
	public class BarometerReader
	{
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
			Text = Data.PressureInHectopascals.ToString(LanguageApi.Current.CultureInfo);
			Value = Data.PressureInHectopascals;
		}

		public void Start() => Start(SensorSpeed.UI);

		public void Start( SensorSpeed speed )
		{
			Speed = speed;
			Barometer.Start(speed);
		}

		public void Stop()
		{
			if ( Barometer.IsMonitoring ) { Barometer.Stop(); }
		}
	}
}
