using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Dynamics;

namespace TDOS.Box2D.Skia
{
    public static class Box2DExtensions
    {
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
