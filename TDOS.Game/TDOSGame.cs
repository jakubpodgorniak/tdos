using Box2DX.Collision;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDOS.Box2D.Skia;
using TDOS.MG.Skia;
using TDOS.MG.Skia.Implementations;
using TDOS.MG.Utils;

namespace TDOS.Game
{
    public class TDOSGame : Microsoft.Xna.Framework.Game
    {
        public TDOSGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            frameRateCounter = new FrameRateCounter(120);

            var worldBox = new AABB();
            worldBox.LowerBound.Set(-100f, -100f);
            worldBox.UpperBound.Set(100f, 100f);
            world = new World(worldBox, new Box2DX.Common.Vec2(0f, 10f), doSleep: false);

            var groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(10f, 15f);
            groundBody = world.CreateBody(groundBodyDef);

            var groundShapeDef = new PolygonDef();
            groundShapeDef.SetAsBox(15, 1);
            groundShapeDef.Friction = .5f;
            groundBody.CreateShape(groundShapeDef);

            var movingBodyDef = new BodyDef();
            movingBodyDef.Position.Set(10f, 0f);
            movingBody = world.CreateBody(movingBodyDef);

            var movingShapeDef = new CircleDef();
            movingShapeDef.LocalPosition = new Box2DX.Common.Vec2(-1f, 0f);
            movingShapeDef.Radius = 1f;
            movingShapeDef.Density = 1f;
            movingShapeDef.Friction = 0.5f;

            var movingShapeDef2 = new CircleDef();
            movingShapeDef2.LocalPosition = new Box2DX.Common.Vec2(1f, 0f);
            movingShapeDef2.Radius = 1f;
            movingShapeDef2.Density = 1f;
            movingShapeDef2.Friction = 0.5f;

            movingBody.CreateShape(movingShapeDef);
            movingBody.CreateShape(movingShapeDef2);
            movingBody.SetMassFromShapes();

            var movingBodyDef2 = new BodyDef();
            movingBodyDef2.Position.Set(12.5f, -4f);
            movingBody2 = world.CreateBody(movingBodyDef2);

            var movingShapeDef3 = new PolygonDef();
            movingShapeDef3.SetAsBox(0.7f, 0.7f);
            movingShapeDef3.Friction = .5f;
            movingShapeDef3.Density = 1f;

            movingBody2.CreateShape(movingShapeDef3);
            movingBody2.SetMassFromShapes();

            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        Body groundBody;
        Body movingBody;
        Body movingBody2;

        protected override void Initialize()
        {
            bitmapToTextureRenderer = new BitmapToTextureRenderer(GraphicsDevice);
            bitmapToTextureRenderer.AddDrawer(new SampleCanvasDrawer());
            bitmapToTextureRenderer.AddDrawer(new RectDrawer());
            bitmapToTextureRenderer.AddDrawer(new WorldDrawer(world, 32));
            bitmapToTextureRenderer.AddDrawer(new FrameRateCounterDrawer(frameRateCounter));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameRateCounter.Update(deltaTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !wasPressed)
            {
                SpawnSquare();

                wasPressed = true;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released && wasPressed)
            {
                wasPressed = false;
            }

            world.Step(deltaTime, 8, 2);

            base.Update(gameTime);
        }

        protected void SpawnSquare()
        {
            var mouseScreenPosition = Mouse.GetState().Position;
            var x = mouseScreenPosition.X / 32;
            var y = mouseScreenPosition.Y / 32;

            var bodyDef = new BodyDef();
            bodyDef.Position.Set(x, y);

            var body = world.CreateBody(bodyDef);

            var shapeDef = new PolygonDef();
            shapeDef.SetAsBox(0.25f, 0.25f);
            shapeDef.Friction = .5f;
            shapeDef.Density = 5f;

            body.CreateShape(shapeDef);
            body.SetMassFromShapes();
        }

        bool wasPressed = false;

        protected override void Draw(GameTime gameTime)
        {
            bitmapToTextureRenderer.Render();

            GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(69, 76, 82, 255));

            spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            spriteBatch.Draw(
                bitmapToTextureRenderer.Texture,
                new Rectangle(0, 0, bitmapToTextureRenderer.Texture.Width, bitmapToTextureRenderer.Texture.Height),
                Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private readonly GraphicsDeviceManager graphics;

        private readonly FrameRateCounter frameRateCounter;

        private BitmapToTextureRenderer bitmapToTextureRenderer;

        private World world;

        private SpriteBatch spriteBatch;
    }
}
