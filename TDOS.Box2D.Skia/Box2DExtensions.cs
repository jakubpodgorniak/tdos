using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using SkiaSharp;

namespace TDOS.Box2D.Skia
{
    public static class Box2DExtensions
    {
        public static SKPoint ToSKPoint(this Vec2 vec2, int scale)
            => new SKPoint(vec2.X * scale, vec2.Y * scale);

        public static Vec2[] GetNonEmptyVertices(this PolygonShape polygonShape)
            => polygonShape.GetVertices().Take(polygonShape.VertexCount).ToArray();

        public static IEnumerable<Body> GetBodies(this World world)
        {
            var body = world.GetBodyList();

            do
            {
                yield return body;

                body = body.GetNext();
            } while (body != null);
        }

        public static IEnumerable<Shape> GetShapes(this Body body)
        {
            var shape = body.GetShapeList();

            if (shape is null) yield break;

            do
            {
                yield return shape;

                shape = shape.GetNext();
            } while (shape != null);
        }
    }
}
