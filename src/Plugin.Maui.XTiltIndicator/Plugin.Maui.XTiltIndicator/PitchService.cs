namespace Plugin.Maui.XTiltIndicator
{
    public class PitchService
    {
        private DateTime _lastUpdate = DateTime.MinValue;
        private const int UpdateIntervalMs = 250;
         
        public event EventHandler<PitchEventArgs>? PitchAngleChangedEvent;

        public void StartAccelerometer()
        {
            if (!Accelerometer.IsMonitoring)
            {
                Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                Accelerometer.Start(SensorSpeed.UI);
            }
        }

        public void StopAccelerometer()
        {
            if (Accelerometer.IsMonitoring)
            {
                Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                Accelerometer.Stop();
            }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastUpdate).TotalMilliseconds < UpdateIntervalMs)
                return;

            _lastUpdate = now;

            var x = e.Reading.Acceleration.X;
            var y = e.Reading.Acceleration.Y;
            var z = e.Reading.Acceleration.Z;


            // === Horizontal Pitch ===
            double horizontalPitch = 0;
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
                horizontalPitch = Math.Atan2(-x, Math.Sqrt(y * y + z * z)) * (180.0 / Math.PI);
            else
                horizontalPitch = Math.Atan2(y, Math.Sqrt(x * x + z * z)) * (180.0 / Math.PI);

            Color pitchColor = Colors.Lime;
            // Set color based on deviation
            if (Math.Abs(horizontalPitch) <= 3)
            {
                pitchColor = Color.FromHex("22A191"); // green 
            }
            else if (Math.Abs(horizontalPitch) <= 10)
            {
                pitchColor = Color.FromHex("E36D01"); // orange 
            }
            else
            {
                pitchColor = Color.FromHex("D5392D"); // red 
            }

            // === Vertical Pitch ===
            double aMagnitude = Math.Sqrt(x * x + y * y + z * z);
            double pMagnitude = Math.Sqrt(x * x + y * y);
            double cosTheta = (x * x + y * y) / (aMagnitude * pMagnitude);
            cosTheta = Math.Clamp(cosTheta, -1, 1);

            double thetaRadians = Math.Acos(cosTheta);
            double thetaDegrees = thetaRadians * (180.0 / Math.PI);
            if (z < 0)
                thetaDegrees = -thetaDegrees;

            PitchAngleChangedEvent?.Invoke(null, new PitchEventArgs
            {
                PitchHorizontal = horizontalPitch,
                PitchLeft = thetaDegrees,
                PitchRight = -thetaDegrees,
                PitchHorizontalColor = pitchColor
            });
        }

    }
}
