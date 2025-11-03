namespace Plugin.Maui.XTiltIndicator.Internal
{
    internal class PitchDrawable : IDrawable
    {
        public double Pitch { get; set; }
        public Color PitchColor { get; set; } = Colors.Lime;

        public double PitchLeft { get; set; }
        public double PitchRight { get; set; }
        public float LeftBarX { get; set; } = 0;
        public float RightBarX { get; set; } = 0;


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            float cx = dirtyRect.Center.X;
            float cy = dirtyRect.Center.Y;

            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            float baseLineY = 0.85f;
            int textFontSize = 14;
            var font = new Microsoft.Maui.Graphics.Font("Equip-Light.otf", 20);
            canvas.Font = font;

            #region Horizontal bars
            canvas.SaveState();
            // === Fixed horizontal reference center line ===
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 0.2f;
            canvas.DrawLine(0, cy, width, cy); // fixed horizontal line
             
            // === Horizontal bar (rotated) ===
            float barWidth = width * 0.2f;
            float barHeight = 2f;
            float rotation = (float)Pitch;

            canvas.FillColor = PitchColor;
            canvas.Translate(cx, cy);
            canvas.Rotate(rotation);
            canvas.FillRoundedRectangle(-barWidth / 2, -barHeight / 2, barWidth, barHeight, 6);

            // === White center point ===
            float dotSize = 4; // point diameter
            canvas.FillColor = Colors.White;
            canvas.FillCircle(0, 0, dotSize / 2);

            canvas.RestoreState();
            #endregion

            #region Vertical Bars
            // === Vertical bars ===
            float vBarWidth = 4;
            float vBarHeight = height * 0.9f;

            // left
            canvas.SaveState();
            canvas.FillColor = Color.FromArgb("#96239E");
            //canvas.Translate(width * 0.05f, cy);
            canvas.Translate(LeftBarX, cy);
            canvas.Rotate((float)PitchLeft);
            canvas.FillRoundedRectangle(-vBarWidth / 2, -vBarHeight / 2, vBarWidth, vBarHeight, 6);
            canvas.RestoreState();

            // right
            canvas.SaveState();
            canvas.FillColor = Color.FromArgb("#96239E");
            //canvas.Translate(width * 0.95f, cy);
            canvas.Translate(RightBarX, cy);
            canvas.Rotate((float)PitchRight);
            canvas.FillRoundedRectangle(-vBarWidth / 2, -vBarHeight / 2, vBarWidth, vBarHeight, 6);
            canvas.RestoreState();

            #endregion


        }
    }
}
