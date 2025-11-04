namespace Plugin.Maui.XTiltIndicator
{
    public class PitchService
    {
        private DateTime _lastUpdate = DateTime.MinValue;
        private const int UpdateIntervalMs = 200;

        private double _filteredX, _filteredY, _filteredZ;
        private const double BaseAlpha = 0.1;   // netezire normală
        private const double FastAlpha = 0.7;   // reacție rapidă la mișcări mari
        private const double ChangeThreshold = 0.03; // prag accelerare (m/s²)

        private double _lastHorizontalPitch = double.NaN;
        private double _lastThetaDegrees = double.NaN;
        private const double DeadbandThreshold = 0.2; // pragul de sensibilitate

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

            var rawX = e.Reading.Acceleration.X;
            var rawY = e.Reading.Acceleration.Y;
            var rawZ = e.Reading.Acceleration.Z;

            // Măsurăm variația totală între frame-uri
            double delta = Math.Sqrt(
                Math.Pow(rawX - _filteredX, 2) +
                Math.Pow(rawY - _filteredY, 2) +
                Math.Pow(rawZ - _filteredZ, 2)
            );

            // Dacă diferența e mare → reacție rapidă, altfel netezită
            double alpha = delta > ChangeThreshold ? FastAlpha : BaseAlpha;

            // Filtru adaptiv
            _filteredX = _filteredX + alpha * (rawX - _filteredX);
            _filteredY = _filteredY + alpha * (rawY - _filteredY);
            _filteredZ = _filteredZ + alpha * (rawZ - _filteredZ);

            var x = _filteredX;
            var y = _filteredY;
            var z = _filteredZ;


            // === Horizontal Pitch ===
            double horizontalPitch = 0;
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
                horizontalPitch = Math.Atan2(-rawX, Math.Sqrt(rawY * rawY + rawZ * rawZ)) * (180.0 / Math.PI);
            else
                horizontalPitch = Math.Atan2(y, Math.Sqrt(rawX * rawX + rawZ * rawZ)) * (180.0 / Math.PI);

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



            // === Aplică deadband pentru stabilitate ===
            if (!double.IsNaN(_lastHorizontalPitch) && !double.IsNaN(_lastThetaDegrees))
            {
                if (Math.Abs(horizontalPitch - _lastHorizontalPitch) < DeadbandThreshold &&
                    Math.Abs(thetaDegrees - _lastThetaDegrees) < DeadbandThreshold)
                {
                    return; // diferență prea mică → ignorăm ca zgomot
                }
            }

            // Actualizează valorile memorate
            _lastHorizontalPitch = horizontalPitch;
            _lastThetaDegrees = thetaDegrees;


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
