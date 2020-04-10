using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.Game
{
    public class RectDrawer : ICanvasDrawer
    {
        public void Draw(SKCanvas cavnas)
        {
            using(var paint = new SKPaint())
            {
                paint.Color = SKColors.Red;

                cavnas.DrawRect(new SKRect(30, 30, 200, 10), paint);
            }
        }
    }
}
