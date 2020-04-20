using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDOS.MG.Skia;
using TDOS.MG.Utils;

namespace TDOS.Game.GameplayLoops
{
    public interface IGameplayLoop
    {
        void Initialize(
            BitmapToTextureRenderer bitmapToTextureRenderer,
            FrameRateCounter frameRateCounter);

        void LoadContent(ContentManager Content);

        void UnloadContent();

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
