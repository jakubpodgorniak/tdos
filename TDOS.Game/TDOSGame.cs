using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDOS.Game.GameplayLoops;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game
{
    public class TDOSGame : Microsoft.Xna.Framework.Game
    {
        public TDOSGame(IGameplayLoop gameplayLoop)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.gameplayLoop = gameplayLoop;

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 450;

            frameRateCounter = new FrameRateCounter(120);

            IsMouseVisible = true;
            IsFixedTimeStep = true;
            //graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            bitmapToTextureRenderer = new BitmapToTextureRenderer(
                GraphicsDevice,
                RenderTargetWidth,
                RenderTargetHeight);

            gameplayLoop.Initialize(
                bitmapToTextureRenderer,
                frameRateCounter);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameplayLoop.LoadContent(Content);

            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        protected override void UnloadContent()
        {
            gameplayLoop.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameRateCounter.Update(deltaTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            gameplayLoop.Update(gameTime);

            base.Update(gameTime);
        }

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
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                new Rectangle(0, 0, RenderTargetWidth, RenderTargetHeight),
                Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawToRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(new Color(69, 76, 82, 255));

            spriteBatch.Begin(blendState: BlendState.AlphaBlend);

            gameplayLoop.Draw(spriteBatch);

            spriteBatch.Draw(
                bitmapToTextureRenderer.Texture,
                new Rectangle(0, 0, bitmapToTextureRenderer.Texture.Width, bitmapToTextureRenderer.Texture.Height),
                Color.White);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        private const int RenderTargetWidth = 800;

        private const int RenderTargetHeight = 450;

        private readonly IGameplayLoop gameplayLoop;

        private readonly GraphicsDeviceManager graphics;

        private readonly FrameRateCounter frameRateCounter;

        private BitmapToTextureRenderer bitmapToTextureRenderer;

        private SpriteBatch spriteBatch;

        private RenderTarget2D renderTarget;
    }
}
