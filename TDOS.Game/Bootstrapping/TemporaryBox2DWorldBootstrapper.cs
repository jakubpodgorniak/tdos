using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace TDOS.Game.Bootstrapping
{
    public static class TemporaryBox2DWorldBootstrapper
    {
        public static World CreateWorld()
        {
            var worldBox = new AABB();
            worldBox.LowerBound.Set(-100f, -100f);
            worldBox.UpperBound.Set(100f, 100f);

            return new World(worldBox, Vec2.Zero, doSleep: false);
        }

        public static Body CreateHeroBody(World world)
        {
            var heroBodyShapeDef = new CircleDef
            {
                LocalPosition = Vec2.Zero,
                Radius = 0.55f,
                Density = 1f
            };
            var heroBodyDef = new BodyDef
            {
                FixedRotation = true,
                LinearDamping = 8.5f
            };
            heroBodyDef.Position.Set(10f, 0f);

            var heroBody = world.CreateBody(heroBodyDef);
            heroBody.CreateShape(heroBodyShapeDef);
            heroBody.SetMassFromShapes();

            return heroBody;
        }
    }
}
