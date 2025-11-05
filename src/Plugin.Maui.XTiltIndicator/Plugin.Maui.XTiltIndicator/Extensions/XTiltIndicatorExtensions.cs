using Plugin.Maui.XTiltIndicator.Services;

namespace Plugin.Maui.XTiltIndicator.Extensions
{
    public static class XTiltIndicatorExtensions
    {
        public static MauiAppBuilder UseXTiltIndicator(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IPitchService, PitchService>();

            return builder;
        }
    }
}
