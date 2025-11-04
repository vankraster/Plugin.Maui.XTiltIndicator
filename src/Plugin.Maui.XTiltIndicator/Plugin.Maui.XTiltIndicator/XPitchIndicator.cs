using Plugin.Maui.XTiltIndicator.Internal;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.XTiltIndicator
{
    public class XPitchIndicator : ContentView
    {
        private readonly GraphicsView _graphicsView;
        private readonly PitchService _pitchService;
        private PitchDrawable _drawable => ((PitchDrawable)_graphicsView.Drawable);

        public event Action<string>? TakePictureHandler;
         
        #region Bindable Properties
        public static readonly BindableProperty HorizontalPitchProperty =
            BindableProperty.Create(nameof(HorizontalPitch), typeof(TPitchType), typeof(TPitchType),
            TPitchType.Gauge,
            propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).HorizontalPitch = (TPitchType)n);

        public TPitchType HorizontalPitch
        {
            get => (TPitchType)GetValue(HorizontalPitchProperty);
            set => SetValue(HorizontalPitchProperty, value);
        }



        public static readonly BindableProperty VerticalPitchProperty =
      BindableProperty.Create(nameof(VerticalPitch), typeof(TPitchType), typeof(TPitchType),
      TPitchType.Gauge,
      propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).VerticalPitch = (TPitchType)n);

        public TPitchType VerticalPitch
        {
            get => (TPitchType)GetValue(VerticalPitchProperty);
            set => SetValue(VerticalPitchProperty, value);
        }
        #endregion
         
        public XPitchIndicator()
        {
            _graphicsView = new GraphicsView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };
            _pitchService = new PitchService();
            _pitchService.PitchAngleChangedEvent += _pitchService_PitchAngleChangedEvent;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                var coord = e.GetPosition((View)s);
                if (coord == null)
                    return;

                var drawable = _graphicsView.Drawable as PitchDrawable;
            };
            _graphicsView.GestureRecognizers.Add(tapGestureRecognizer);

            _graphicsView.Drawable = new PitchDrawable();

            Content = _graphicsView;
        }

        private void _pitchService_PitchAngleChangedEvent(object? sender, PitchEventArgs e)
        {
            _drawable.PitchLeft = e.PitchLeft;
            _drawable.PitchRight = e.PitchRight;
            _drawable.Pitch = e.PitchHorizontal;

            _graphicsView.Invalidate();
        }

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

                _graphicsView.Invalidate();
            }
        }
          
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(VerticalPitch) || propertyName == nameof(HorizontalPitch))
                _graphicsView.Invalidate();
        }
    }
}
