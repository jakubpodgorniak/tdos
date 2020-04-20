using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDOS.Box2D.Skia;
using TDOS.Game.Bootstrapping;
using TDOS.Game.Characters;
using TDOS.Game.Configuration.Helpers;
using TDOS.Game.Rendering;
using TDOS.Game.Resources;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game.GameplayLoops.Concrete
{
    public class Standalone : IGameplayLoop
    {
        public Standalone(Configuration.Configuration configuration)
        {
            this.configuration = configuration;
            bodySprites = new List<BodySprite>();
        }

        public void Initialize(
            BitmapToTextureRenderer bitmapToTextureRenderer,
            FrameRateCounter frameRateCounter)
        {
            world = TemporaryBox2DWorldBootstrapper.CreateWorld();
            var heroBody = TemporaryBox2DWorldBootstrapper.CreateHeroBody(world);
            hero = new Character(heroBody)
            {
                Acceleration = 80f,
                MaxSpeed = 8f
            };

            bitmapToTextureRenderer.AddDrawer(Drawers.Colliders, new WorldDrawer(world, Constants.PixelsPerUnit));
            bitmapToTextureRenderer.AddDrawer(Drawers.BodiesStatus, new BodiesStatusDrawer(world, Constants.PixelsPerUnit) { DisplayVelocity = true });
            bitmapToTextureRenderer.AddDrawer(Drawers.FpsCounter, new FrameRateCounterDrawer(frameRateCounter));

            var visiblityController = new DebugInterfaceVisibilityController(bitmapToTextureRenderer);
            visiblityController.RefreshVisibility(configuration.DebugInterface);

            var configurationWatcher = new ConfigurationWatcher(
                @".\Resources\Configuration.json",
                config => visiblityController.RefreshVisibility(config.DebugInterface));
        }

        public void LoadContent(ContentManager Content)
        {
            crateTexture = Content.Load<Texture2D>("crate");

            bodySprites.Add(new BodySprite(
                Content.Load<Texture2D>("hero"),
                hero.Body,
                Constants.PixelsPerUnit,
                new Vector2(18, 22)));
        }

        public void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                activeDirections.Add(MoveDirection.Up);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                activeDirections.Add(MoveDirection.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                activeDirections.Add(MoveDirection.Down);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                activeDirections.Add(MoveDirection.Left);
            }

            var mousePosition = Mouse.GetState().Position;
            var mousePositionVec = new Vec2(mousePosition.X / (2f * Constants.PixelsPerUnit), mousePosition.Y / (2f * Constants.PixelsPerUnit));

            hero.UpdatePosition(activeDirections.ToArray());
            hero.UpdateRotation(mousePositionVec);

            world.Step(deltaTime, 6, 2);
        }

        bool wasPressed;

        protected void SpawnSquare()
        {
            var mouseScreenPosition = Mouse.GetState().Position;
            var x = (mouseScreenPosition.X / 2) / Constants.PixelsPerUnit;
            var y = (mouseScreenPosition.Y / 2) / Constants.PixelsPerUnit;

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
                Constants.PixelsPerUnit));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bodySprite in bodySprites.Reverse())
            {
                bodySprite.Render(spriteBatch);
            }
        }

        private readonly Configuration.Configuration configuration;

        private readonly IList<BodySprite> bodySprites;

        private Texture2D crateTexture;

        private World world;

        private Character hero;
    }
}
