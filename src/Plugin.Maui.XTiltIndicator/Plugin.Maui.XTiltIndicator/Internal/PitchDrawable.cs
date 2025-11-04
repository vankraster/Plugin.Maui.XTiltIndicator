using Microsoft.Maui.Graphics;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.XTiltIndicator.Internal
{
    internal class PitchDrawable : IDrawable
    {
        public TPitchType HorizontalPitch { get; set; } = TPitchType.Gauge;
        public TPitchType VerticalPitch { get; set; } = TPitchType.Gauge;


        public double Pitch { get; set; }

        public double PitchLeft { get; set; }
        public double PitchRight { get; set; }
        public float LeftBarX { get; set; } = 0;
        public float RightBarX { get; set; } = 0;

        public Color PitchColor { get; set; } = Colors.Lime;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cx = dirtyRect.Center.X;
            float cy = dirtyRect.Center.Y;

            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            #region Horizontal bars
            canvas.SaveState();
            // === Fixed horizontal reference center line ===
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 0.2f;
            canvas.DrawLine(0, cy, width, cy); // fixed horizontal line

            if (HorizontalPitch == TPitchType.Bars)
            {
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
            }

            canvas.RestoreState();
            #endregion

            #region Vertical Bars
            if (VerticalPitch == TPitchType.Bars)
            {
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
            }
            #endregion

            #region Horizontal Pitch Arc (baza pe linia orizontală)
            if (HorizontalPitch == TPitchType.Gauge)
            {
                canvas.SaveState();

                // Raza semicercului mai mică și baza lipită de linia centrală
                float arcRadius = Math.Min(width, height) * 0.25f;
                float arcCenterX = cx;
                float arcCenterY = cy; // centrul semicercului chiar pe linia fixă

                // === Arcul semicircular 180° ===
                canvas.StrokeColor = Colors.Gray.WithAlpha(0.5f);
                canvas.StrokeSize = 2f;
                canvas.DrawArc(
                    arcCenterX - arcRadius,
                    arcCenterY - arcRadius,
                    arcRadius * 2,
                    arcRadius * 2,
                    180, // startAngle (stânga)
                    180, // sweepAngle (spre dreapta)
                    false,
                    false
                );

                // Marcaje la fiecare 5°, mai lungi la multipli de 10°
                for (int deg = -90; deg <= 90; deg += 5)
                {
                    double rad = (180 - (deg + 90)) * Math.PI / 180.0;
                    float innerR = arcRadius - (deg % 10 == 0 ? 10 : 5);
                    float outerR = arcRadius;
                    float sin = (float)Math.Sin(rad);
                    float cos = (float)Math.Cos(rad);

                    float x1 = arcCenterX + innerR * cos;
                    float y1 = arcCenterY - innerR * sin;
                    float x2 = arcCenterX + outerR * cos;
                    float y2 = arcCenterY - outerR * sin;

                    canvas.StrokeColor = deg % 10 == 0 ? Colors.Lime : Colors.Gray.WithAlpha(0.6f);
                    canvas.StrokeSize = deg % 10 == 0 ? 2f : 1f;
                    canvas.DrawLine(x1, y1, x2, y2);

                    // text la multipli de 30°
                    if (deg % 30 == 0)
                    {
                        string label = deg.ToString("+#;-#;0");
                        float tx = arcCenterX + (arcRadius - 20) * cos;
                        float ty = arcCenterY - (arcRadius - 20) * sin;

                        canvas.FontColor = Colors.Gray;
                        canvas.FontSize = 12;
                        canvas.DrawString(label, tx - 10, ty - 8, 20, 16, HorizontalAlignment.Center, VerticalAlignment.Center);
                    }
                }

                // === Indicatorul curent (Pitch) ===
                float pitchClamped = (float)Math.Clamp(Pitch, -90, 90);
                double pitchRad = (180 - (pitchClamped + 90)) * Math.PI / 180.0;
                float indicatorLen = arcRadius;
                float ix = arcCenterX + indicatorLen * (float)Math.Cos(pitchRad);
                float iy = arcCenterY - indicatorLen * (float)Math.Sin(pitchRad);

                canvas.StrokeColor = PitchColor;
                canvas.StrokeSize = 3f;
                canvas.DrawLine(arcCenterX, arcCenterY, ix, iy);

                // === Valoarea numerică Pitch deasupra semicercului ===
                canvas.FontColor = PitchColor;
                canvas.FontSize = 16;
                string pitchText = $"{Pitch:F1}°";
                canvas.DrawString(
                    pitchText,
                    arcCenterX - 30,
                    arcCenterY - arcRadius - 20,
                    60,
                    20,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center
                );

                canvas.RestoreState();
            }
            #endregion

            #region Vertical Scale (PitchRight) 
            if (VerticalPitch == TPitchType.Gauge)
            {
                // configurare
                float spacing = 20f;     // distanța între valorile întregi 
                float baseValue = (float)PitchRight;

                // calculăm intervalul vizibil
                float range = height / (spacing * 2);
                float minValue = baseValue - range;
                float maxValue = baseValue + range;

                // coordonata fixă X (în dreapta)
                float x = RightBarX;

                // desenăm scala
                for (float v = MathF.Floor(minValue * 2) / 2; v <= maxValue; v += 0.5f)
                {
                    // Poziția verticală pentru fiecare marcaj
                    float dy = (v - baseValue) * spacing * 2;
                    float y = cy - dy;

                    bool isMain = Math.Abs(v % 1) < 0.001f; // marcaj principal la fiecare 1 grad

                    float lineLength = isMain ? 25f : 10f;
                    float lineWidth = isMain ? 2f : 1f;
                    Color lineColor = isMain ? Colors.Lime : Colors.Gray.WithAlpha(0.7f);

                    // linie orizontală simplă
                    canvas.StrokeColor = lineColor;
                    canvas.StrokeSize = lineWidth;
                    canvas.DrawLine(x - lineLength, y, x, y);

                    // text doar pentru valorile principale
                    if (isMain)
                    {
                        string label = v.ToString("+#;-#;0");
                        float textWidth = 30;
                        float textHeight = 14;

                        canvas.FontColor = Colors.Gray;
                        canvas.DrawString(
                            label,
                            x - lineLength - textWidth,
                            y - textHeight / 2,
                            textWidth,
                            textHeight,
                            HorizontalAlignment.Right,
                            VerticalAlignment.Center
                        );
                    }
                }

                // linia centrală care arată valoarea actuală
                //canvas.StrokeColor = Colors.Red;
                //canvas.StrokeSize = 2f;
                //canvas.DrawLine(x - 30, cy, x + 10, cy);
            }
            #endregion

        }
    }
}
