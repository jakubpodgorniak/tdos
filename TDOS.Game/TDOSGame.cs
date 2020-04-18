using System.Collections.Generic;
using System.IO;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using TDOS.Box2D.Skia;
using TDOS.Game.Characters;
using TDOS.Game.Configuration.Helpers;
using TDOS.Game.Rendering;
using TDOS.Game.Resources;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game
{
    public class TDOSGame : Microsoft.Xna.Framework.Game
    {
        public TDOSGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;

            frameRateCounter = new FrameRateCounter(120);

            var worldBox = new AABB();
            worldBox.LowerBound.Set(-100f, -100f);
            worldBox.UpperBound.Set(100f, 100f);
            world = new World(worldBox, Vec2.Zero, doSleep: false);

            var groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(10f, 13f);
            groundBody = world.CreateBody(groundBodyDef);

            var groundShapeDef = new PolygonDef();
            groundShapeDef.SetAsBox(15, 1);
            groundBody.CreateShape(groundShapeDef);

            var movingBodyDef = new BodyDef();
            movingBodyDef.Position.Set(10f, 0f);
            movingBodyDef.FixedRotation = true;
            movingBodyDef.LinearDamping = 8.5f;
            var movingBody = world.CreateBody(movingBodyDef);

            var movingShapeDef = new CircleDef();
            movingShapeDef.LocalPosition = Vec2.Zero;
            movingShapeDef.Radius = 0.55f;
            movingShapeDef.Density = 1f;

            movingBody.CreateShape(movingShapeDef);
            movingBody.SetMassFromShapes();

            hero = new Character(movingBody)
            {
                Acceleration = 80f,
                MaxSpeed = 8f // m/s
            };

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            configuration = JsonConvert.DeserializeObject<Configuration.Configuration>(
                File.ReadAllText(@"Resources\Configuration.json"));

            //graphics.IsFullScreen = true;

            bodySprites = new List<BodySprite>();
        }

        Body groundBody;

        protected override void Initialize()
        {
            bitmapToTextureRenderer = new BitmapToTextureRenderer(GraphicsDevice, RenderTargetWidth, RenderTargetHeight);
            bitmapToTextureRenderer.AddDrawer(Drawers.Colliders, new WorldDrawer(world, PixelsPerUnit));
            bitmapToTextureRenderer.AddDrawer(Drawers.BodiesStatus, new BodiesStatusDrawer(world, PixelsPerUnit) { DisplayVelocity = true });
            bitmapToTextureRenderer.AddDrawer(Drawers.FpsCounter, new FrameRateCounterDrawer(frameRateCounter));

            var visiblityController = new DebugInterfaceVisibilityController(bitmapToTextureRenderer);
            visiblityController.RefreshVisibility(configuration.DebugInterface);

            var configurationWatcher = new ConfigurationWatcher(
                @".\Resources\Configuration.json",
                config => visiblityController.RefreshVisibility(config.DebugInterface));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            crateTexture = Content.Load<Texture2D>("crate");

            bodySprites.Add(new BodySprite(
                Content.Load<Texture2D>("hero"),
                hero.Body,
                PixelsPerUnit,
                new Vector2(18, 22)));

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        protected override void UnloadContent() { }

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

            var activeDirections = new List<MoveDirection>();

            var mousePosition = Mouse.GetState().Position;
            var mousePositionVec = new Vec2(mousePosition.X / (2f * PixelsPerUnit), mousePosition.Y / (2f * PixelsPerUnit));

            if (Keyboard.GetState().IsKeyDown(Keys.W)) activeDirections.Add(MoveDirection.Up);
            if (Keyboard.GetState().IsKeyDown(Keys.D)) activeDirections.Add(MoveDirection.Right);
            if (Keyboard.GetState().IsKeyDown(Keys.S)) activeDirections.Add(MoveDirection.Down);
            if (Keyboard.GetState().IsKeyDown(Keys.A)) activeDirections.Add(MoveDirection.Left);

            hero.UpdatePosition(activeDirections.ToArray());
            hero.UpdateRotation(mousePositionVec);

            world.Step(deltaTime, 6, 2);

            base.Update(gameTime);
        }

        protected void SpawnSquare()
        {
            var mouseScreenPosition = Mouse.GetState().Position;
            var x = (mouseScreenPosition.X / 2) / PixelsPerUnit;
            var y = (mouseScreenPosition.Y / 2) / PixelsPerUnit;

            var bodyDef = new BodyDef();
            bodyDef.FixedRotation = true;
            bodyDef.LinearDamping = 8f;
            bodyDef.Position.Set(x, y);

            var body = world.CreateBody(bodyDef);

            var shapeDef = new PolygonDef();
            shapeDef.SetAsBox(0.5f, 0.5f);
            shapeDef.Friction = .5f;
            shapeDef.Density = 5f;

            body.CreateShape(shapeDef);
            body.SetMassFromShapes();

            bodySprites.Add(new BodySprite(
                crateTexture,
                body,
                PixelsPerUnit));
        }

        bool wasPressed = false;

        protected override void Draw(GameTime gameTime)
        {
            bitmapToTextureRenderer.Render();

            DrawToRenderTarget();

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null);

            spriteBatch.Draw(
                renderTarget,
                new Rectangle(0, 0, 2 * RenderTargetWidth, 2 * RenderTargetHeight),
                new Rectangle(0, 0, RenderTargetWidth, RenderTargetHeight),
                Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawToRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(69, 76, 82, 255));

            spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            foreach (var bodySprite in bodySprites)
            {
                bodySprite.Render(spriteBatch);
            }

            spriteBatch.Draw(
                bitmapToTextureRenderer.Texture,
                new Rectangle(0, 0, bitmapToTextureRenderer.Texture.Width, bitmapToTextureRenderer.Texture.Height),
                Microsoft.Xna.Framework.Color.White);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        private const int PixelsPerUnit = 32;

        private const int RenderTargetWidth = 800;

        private const int RenderTargetHeight = 450;

        private readonly GraphicsDeviceManager graphics;

        private readonly FrameRateCounter frameRateCounter;

        private readonly Configuration.Configuration configuration;

        private BitmapToTextureRenderer bitmapToTextureRenderer;

        private World world;

        private SpriteBatch spriteBatch;

        private RenderTarget2D renderTarget;

        private Texture2D crateTexture;

        private IList<BodySprite> bodySprites;

        private Character hero;
    }
}
