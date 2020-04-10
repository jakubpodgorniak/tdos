using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.MG.Utils
{
    public class FrameRateCounterDrawer : ICanvasDrawer
    {
        public FrameRateCounterDrawer(FrameRateCounter frameRateCounter)
        {
            this.frameRateCounter = frameRateCounter;
        }

        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.White;

                canvas.DrawText(
                    $"FPS: {frameRateCounter.CurrentFramesPerSecond}",
                    new SKPoint(15, 15),
                    paint);

                canvas.DrawText(
                    $"AVG.FPS: {frameRateCounter.CurrentFramesPerSecond}",
                    new SKPoint(15, 45),
                    paint);
            }
        }

        private readonly FrameRateCounter frameRateCounter;
    }
}
