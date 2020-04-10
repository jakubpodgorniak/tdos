using SkiaSharp;

namespace TDOS.MG.Skia.Implementations
{
    public class SampleCanvasDrawer : ICanvasDrawer
    {
        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.Purple;

                canvas.DrawCircle(new SKPoint(50, 50), 30f, paint);
            }
        }
    }
}
