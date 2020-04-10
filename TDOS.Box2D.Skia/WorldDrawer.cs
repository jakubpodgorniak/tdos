using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using SkiaSharp;
using TDOS.MG.Skia;

namespace TDOS.Box2D.Skia
{
    public class WorldDrawer : ICanvasDrawer
    {
        public WorldDrawer(World world, int pixelsPerUnit)
        {
            this.world = world;
            this.pixelsPerUnit = pixelsPerUnit;
            CollidersColor = SKColors.LightGreen;
            CollidersWidth = 2;
        }

        public SKColor CollidersColor { get; set; }

        public int CollidersWidth { get; set; }

        public void Draw(SKCanvas canvas)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = CollidersColor;
                paint.StrokeWidth = CollidersWidth;
                paint.Style = SKPaintStyle.Stroke;

                foreach (var shape in world.GetBodies().SelectMany(body => body.GetShapes()))
                {
                    var body = shape.GetBody();

                    if (shape is CircleShape circleShape)
                    {
                        canvas.DrawCircle(
                            body.GetWorldPoint(circleShape.GetLocalPosition()).ToSKPoint(pixelsPerUnit),
                            circleShape.GetRadius() * pixelsPerUnit,
                            paint);
                    }
                    else if (shape is PolygonShape polygonShape)
                    {
                        canvas.DrawPath(
                            GeneratePolygonPath(body, polygonShape.GetNonEmptyVertices()),
                            paint);
                    }
                }
            }
        }

        private SKPath GeneratePolygonPath(Body body, Vec2[] vertices)
        {
            var path = new SKPath();
            var firstPoint = body.GetWorldPoint(vertices[0]).ToSKPoint(pixelsPerUnit);

            path.MoveTo(firstPoint);
            foreach (var vertex in vertices.Skip(1))
            {
                path.LineTo(body.GetWorldPoint(vertex).ToSKPoint(pixelsPerUnit));
            }

            path.LineTo(firstPoint);
            path.Close();

            return path;
        }

        private readonly World world;
        private readonly int pixelsPerUnit;
    }
}
