﻿using System.Collections.Generic;
using System.IO;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using TDOS.Box2D.Skia;
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

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            frameRateCounter = new FrameRateCounter(120);

            var worldBox = new AABB();
            worldBox.LowerBound.Set(-100f, -100f);
            worldBox.UpperBound.Set(100f, 100f);
            world = new World(worldBox, Box2DX.Common.Vec2.Zero, doSleep: false);

            var groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(10f, 15f);
            groundBody = world.CreateBody(groundBodyDef);

            var groundShapeDef = new PolygonDef();
            groundShapeDef.SetAsBox(15, 1);
            groundShapeDef.Friction = .5f;
            groundBody.CreateShape(groundShapeDef);

            var movingBodyDef = new BodyDef();
            movingBodyDef.Position.Set(10f, 0f);
            movingBodyDef.FixedRotation = true;
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

            configuration = JsonConvert.DeserializeObject<Configuration.Configuration>(
                File.ReadAllText(@"Resources\Configuration.json"));

            //graphics.IsFullScreen = true;

            bodySprites = new List<BodySprite>();
        }

        Body groundBody;
        Body movingBody;
        Body movingBody2;

        protected override void Initialize()
        {
            bitmapToTextureRenderer = new BitmapToTextureRenderer(GraphicsDevice);
            bitmapToTextureRenderer.AddDrawer(Drawers.Colliders, new WorldDrawer(world, PixelsPerUnit));
            bitmapToTextureRenderer.AddDrawer(Drawers.BodiesPosition, new BodiesPositionDrawer(world, PixelsPerUnit));
            bitmapToTextureRenderer.AddDrawer(Drawers.FpsCounter, new FrameRateCounterDrawer(frameRateCounter));

            var visiblityController = new DebugInterfaceVisibilityController(bitmapToTextureRenderer);
            visiblityController.RefreshVisibility(configuration.DebugInterface);

            configurationWatcher = new ConfigurationWatcher(
                @".\Resources\Configuration.json",
                config => visiblityController.RefreshVisibility(config.DebugInterface));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            crateTexture = Content.Load<Texture2D>("crate");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            world.Step(deltaTime, 8, 2);
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

            const float MaxSpeed = 10f;
            const float Acc = 450f;
            var velocity = movingBody.GetLinearVelocity();

            var isMovingLeft = Keyboard.GetState().IsKeyDown(Keys.A);
            var isMovingRight = Keyboard.GetState().IsKeyDown(Keys.D);
            var isMovingUp = Keyboard.GetState().IsKeyDown(Keys.W);
            var isMovingDown = Keyboard.GetState().IsKeyDown(Keys.S);

            movingBody.ApplyForce(velocity * -50f, movingBody.GetWorldCenter());

            var force = new Box2DX.Common.Vec2(0, 0);
            if (isMovingLeft && !isMovingRight)
            {
                force.Set(-Acc, 0);
            }
            else if (isMovingRight && !isMovingLeft)
            {
                force.Set(Acc, 0);
            }

            if (isMovingUp && !isMovingDown)
            {
                force.Set(force.X, -Acc);
            }
            else if (isMovingDown && !isMovingUp)
            {
                force.Set(force.X, Acc);
            }

            force.Normalize();
            force *= Acc;

            if (velocity.Length() > MaxSpeed)
            {
                var normalizedVelocity = new Vec2(velocity.X, velocity.Y);
                normalizedVelocity.Length();

                normalizedVelocity *= force.Length();

                movingBody.ApplyForce(force - normalizedVelocity, movingBody.GetWorldCenter());
            }
            else
            {
                movingBody.ApplyForce(force, movingBody.GetWorldCenter());
            }

            base.Update(gameTime);
        }

        protected void SpawnSquare()
        {
            var mouseScreenPosition = Mouse.GetState().Position;
            var x = mouseScreenPosition.X / PixelsPerUnit;
            var y = mouseScreenPosition.Y / PixelsPerUnit;

            var bodyDef = new BodyDef();
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

            base.Draw(gameTime);
        }

        private const int PixelsPerUnit = 32;

        private readonly GraphicsDeviceManager graphics;

        private readonly FrameRateCounter frameRateCounter;

        private readonly Configuration.Configuration configuration;

        private BitmapToTextureRenderer bitmapToTextureRenderer;

        private ConfigurationWatcher configurationWatcher;

        private World world;

        private SpriteBatch spriteBatch;

        private Texture2D crateTexture;

        private IList<BodySprite> bodySprites;
    }
}
