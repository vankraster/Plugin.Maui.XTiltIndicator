using Plugin.Maui.XTiltIndicator.Models;

namespace Plugin.Maui.XTiltIndicator.Services
{
    public interface IPitchService
    {
        public event EventHandler<PitchEventArgs>? PitchAngleChangedEvent;
        void StopAccelerometer();
        void StartAccelerometer();
    }
}
