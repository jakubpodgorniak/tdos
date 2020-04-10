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
        }

        public void Draw(SKCanvas canvas)
        {
            var bodies = world.GetBodies();

            using (var paint = new SKPaint())
            {
                paint.Color = SKColors.LightGreen;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 2;

                foreach (var body in bodies)
                {
                    var bodyPosition = body.GetPosition();
                    var bodyAngle = body.GetAngle();
                    var shapes = body.GetShapes().ToArray();

                    foreach (var shape in shapes)
                    {
                        if (shape.GetType() == ShapeType.CircleShape)
                        {
                            var circleShape = shape as CircleShape;

                            var shapeWorldPositon = body.GetWorldPoint(circleShape.GetLocalPosition());
                            var point = new SKPoint(shapeWorldPositon.X * pixelsPerUnit, shapeWorldPositon.Y * pixelsPerUnit);

                            canvas.DrawCircle(
                                point,
                                circleShape.GetRadius() * pixelsPerUnit,
                                paint);
                        }
                        else if (shape.GetType() == ShapeType.PolygonShape)
                        {
                            var polygonShape = shape as PolygonShape;

                            var vertices = polygonShape.GetVertices().Take(polygonShape.VertexCount).ToArray();

                            DrawPath(canvas, paint, body, vertices);
                        }
                    }
                }
            }
        }

        private void DrawPath(SKCanvas canvas, SKPaint paint, Body body, Vec2[] vertices)
        {
            var path = new SKPath();

            var firstVertex = body.GetWorldPoint(vertices[0]);
            var firstPoint = new SKPoint(firstVertex.X * pixelsPerUnit, firstVertex.Y * pixelsPerUnit);

            path.MoveTo(firstPoint);

            foreach (var vertex in vertices.Skip(1))
            {
                var v = body.GetWorldPoint(vertex);
                var point = new SKPoint(v.X * pixelsPerUnit, v.Y * pixelsPerUnit);

                path.LineTo(point);
            }

            path.LineTo(firstPoint);

            path.Close();

            canvas.DrawPath(path, paint);
        }

        private readonly World world;
        private readonly int pixelsPerUnit;
    }
}
