using Box2DX.Common;
using Box2DX.Dynamics;
using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.Box2D.Skia
{
    public class BodiesStatusDrawer : ICanvasDrawer
    {
        public BodiesStatusDrawer(World world, int pixelsPerUnit)
        {
            this.world = world;
            this.pixelsPerUnit = pixelsPerUnit;
            TextColor = SKColors.Pink;
        }

        public SKColor TextColor { get; set; }

        public bool DisplayVelocity { get; set; }

        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = TextColor;

                foreach (var body in world.GetBodies())
                {
                    var bodyPosition = body.GetPosition();
                    var screenPosition = bodyPosition.ToSKPoint(pixelsPerUnit);

                    canvas.DrawText(
                        GeneratePositionText(bodyPosition),
                        screenPosition,
                        paint);

                    var velocityPosition = new SKPoint(screenPosition.X, screenPosition.Y + 16);
                    canvas.DrawText(
                        GenerateVelocityText(body.GetLinearVelocity()),
                        velocityPosition,
                        paint);
                }
            }
        }

        private string GeneratePositionText(Vec2 position)
            => $"[{position.X.ToString(VectorCoordinateFormat)}: {position.Y.ToString(VectorCoordinateFormat)}]";

        private string GenerateVelocityText(Vec2 velocity)
            => $"velocity: [{velocity.X.ToString(VectorCoordinateFormat)}: {velocity.Y.ToString(VectorCoordinateFormat)} ], "
                + velocity.Length().ToString("#.##");

        private const string VectorCoordinateFormat = "#.##";

        private readonly World world;
        private readonly int pixelsPerUnit;
    }
}
