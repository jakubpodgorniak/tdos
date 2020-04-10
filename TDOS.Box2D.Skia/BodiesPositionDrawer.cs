using Box2DX.Common;
using Box2DX.Dynamics;
using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.Box2D.Skia
{
    public class BodiesPositionDrawer : ICanvasDrawer
    {
        public BodiesPositionDrawer(World world, int pixelsPerUnit)
        {
            this.world = world;
            this.pixelsPerUnit = pixelsPerUnit;
            TextColor = SKColors.Pink;
        }

        public SKColor TextColor { get; set; }

        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = TextColor;

                foreach (var body in world.GetBodies())
                {
                    var bodyPosition = body.GetPosition();

                    canvas.DrawText(
                        GeneratePositionText(bodyPosition),
                        bodyPosition.ToSKPoint(pixelsPerUnit),
                        paint);
                }
            }
        }

        private string GeneratePositionText(Vec2 position)
            => $"[{position.X.ToString(PositionCoordinateFormat)}, {position.Y.ToString(PositionCoordinateFormat)}]";

        private const string PositionCoordinateFormat = "#.##";

        private readonly World world;
        private readonly int pixelsPerUnit;
    }
}
