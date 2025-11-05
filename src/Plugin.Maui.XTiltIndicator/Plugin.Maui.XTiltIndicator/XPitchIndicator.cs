using Plugin.Maui.XTiltIndicator.Internal;
using Plugin.Maui.XTiltIndicator.Models;
using Plugin.Maui.XTiltIndicator.Services;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.XTiltIndicator
{
    public class XPitchIndicator : GraphicsView
    { 
        private readonly PitchService _pitchService;
        private PitchDrawable _drawable => ((PitchDrawable)Drawable);

        public event Action<string>? TakePictureHandler;

        #region Bindable Properties
        public static readonly BindableProperty HorizontalPitchProperty =
            BindableProperty.Create(nameof(HorizontalPitch), typeof(TPitchType), typeof(XPitchIndicator),
            TPitchType.Gauge,
            propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b).Drawable).HorizontalPitch = (TPitchType)n);

        public TPitchType HorizontalPitch
        {
            get => (TPitchType)GetValue(HorizontalPitchProperty);
            set => SetValue(HorizontalPitchProperty, value);
        }



        public static readonly BindableProperty VerticalPitchProperty =
      BindableProperty.Create(nameof(VerticalPitch), typeof(TPitchType), typeof(XPitchIndicator),
      TPitchType.Gauge,
      propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b).Drawable).VerticalPitch = (TPitchType)n);

        public TPitchType VerticalPitch
        {
            get => (TPitchType)GetValue(VerticalPitchProperty);
            set => SetValue(VerticalPitchProperty, value);
        }


        public static readonly BindableProperty PitchColorProperty =
              BindableProperty.Create(nameof(PitchColor), typeof(Color), typeof(XPitchIndicator),
              Colors.Lime,
              propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b).Drawable).PitchColor = (Color)n);

        public Color PitchColor
        {
            get => (Color)GetValue(PitchColorProperty);
            set => SetValue(PitchColorProperty, value);
        }


        public static readonly BindableProperty PitchZeroColorProperty =
              BindableProperty.Create(nameof(PitchZeroColor), typeof(Color), typeof(XPitchIndicator),
              Colors.Red,
              propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b).Drawable).PitchZeroColor = (Color)n);

        public Color PitchZeroColor
        {
            get => (Color)GetValue(PitchZeroColorProperty);
            set => SetValue(PitchZeroColorProperty, value);
        }
        #endregion

        public XPitchIndicator()
        { 
            _pitchService = new PitchService();
            _pitchService.PitchAngleChangedEvent += _pitchService_PitchAngleChangedEvent;

            //TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            //tapGestureRecognizer.Tapped += async (s, e) =>
            //{
            //    var coord = e.GetPosition((View)s);
            //    if (coord == null)
            //        return;

            //    var drawable = Drawable as PitchDrawable;
            //};
            //_graphicsView.GestureRecognizers.Add(tapGestureRecognizer);

            Drawable = new PitchDrawable(); 
        }

        private void _pitchService_PitchAngleChangedEvent(object? sender, PitchEventArgs e)
        {
            const double smoothing = 0.15; // 0.1–0.2 e bine

            _drawable.PitchLeft = Lerp(_drawable.PitchLeft, e.PitchLeft, smoothing);
            _drawable.PitchRight = Lerp(_drawable.PitchRight, e.PitchRight, smoothing);
            _drawable.Pitch = Lerp(_drawable.Pitch, e.PitchHorizontal, smoothing);

            Invalidate(); // sau InvalidateDrawable(_drawable);
        }

        private static double Lerp(double from, double to, double t)
            => from + (to - from) * t;

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null)
            {
                _pitchService.StartAccelerometer();
            }

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0)
            {
                var padding = width * 0.10f;

                _drawable.LeftBarX = (float)(padding);
                _drawable.RightBarX = (float)(width - padding);

                Invalidate();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(VerticalPitch) || propertyName == nameof(HorizontalPitch) || propertyName == nameof(PitchColor))
                Invalidate();
        }
    }
}
