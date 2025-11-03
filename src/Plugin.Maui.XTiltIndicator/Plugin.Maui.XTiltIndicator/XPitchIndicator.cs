using Plugin.Maui.XTiltIndicator.Internal;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.XTiltIndicator
{
    public class XPitchIndicator : ContentView
    {
        private readonly GraphicsView _graphicsView;
        private readonly PitchService _pitchService;

        public event Action<string>? TakePictureHandler;

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
            PitchColor = e.PitchHorizontalColor;
            PitchLeft = e.PitchLeft;
            PitchRight = e.PitchRight;
            Pitch = e.PitchHorizontal;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null)
            {
                _pitchService.StartAccelerometer();
            }

        }

        public static readonly BindableProperty PitchProperty =
            BindableProperty.Create(nameof(Pitch), typeof(double), typeof(XPitchIndicator),
                0.0,
                propertyChanged: (b, o, n) =>
                ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).Pitch = (double)n);

        public double Pitch
        {
            get => (double)GetValue(PitchProperty);
            set => SetValue(PitchProperty, value);
        }

        public static readonly BindableProperty PitchColorProperty =
            BindableProperty.Create(nameof(PitchColor), typeof(Color), typeof(XPitchIndicator),
                Colors.Lime, propertyChanged: (b, o, n) =>
                ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).PitchColor = (Color)n);

        public Color PitchColor
        {
            get => (Color)GetValue(PitchColorProperty);
            set => SetValue(PitchColorProperty, value);
        }

        public static readonly BindableProperty PitchLeftProperty =
            BindableProperty.Create(nameof(PitchLeft), typeof(double), typeof(XPitchIndicator),
                0.0, propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).PitchLeft = (double)n);

        public double PitchLeft
        {
            get => (double)GetValue(PitchLeftProperty);
            set => SetValue(PitchLeftProperty, value);
        }

        public static readonly BindableProperty PitchRightProperty =
            BindableProperty.Create(nameof(PitchRight), typeof(double), typeof(XPitchIndicator),
                0.0, propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).PitchRight = (double)n);

        public double PitchRight
        {
            get => (double)GetValue(PitchRightProperty);
            set => SetValue(PitchRightProperty, value);
        }




        public static readonly BindableProperty LeftBarXProperty =
        BindableProperty.Create(nameof(LeftBarX), typeof(float), typeof(XPitchIndicator),
            0.0f, propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).LeftBarX = (float)n);

        public float LeftBarX
        {
            get => (float)GetValue(LeftBarXProperty);
            set => SetValue(LeftBarXProperty, value);
        }



        public static readonly BindableProperty RightBarXProperty =
        BindableProperty.Create(nameof(RightBarX), typeof(float), typeof(XPitchIndicator),
            0.0f, propertyChanged: (b, o, n) => ((PitchDrawable)((XPitchIndicator)b)._graphicsView.Drawable).RightBarX = (float)n);

        public float RightBarX
        {
            get => (float)GetValue(RightBarXProperty);
            set => SetValue(RightBarXProperty, value);
        }






        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Pitch))
                _graphicsView.Invalidate();
        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0)
            {
                var padding = width * 0.10f;

                LeftBarX = (float)(padding);
                RightBarX = (float)(width - padding);
            }
        }

    }
}
