using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDOS.Game.Rendering
{
    public class BodySprite
    {
        public BodySprite(Texture2D texture, Body body, int pixelsPerUnit)
        {
            this.texture = texture;
            this.body = body;
            this.pixelsPerUnit = pixelsPerUnit;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            var texWidth = texture.Width;
            var texHeight = texture.Height;
            var bodyPosition = body.GetPosition();

            var x = bodyPosition.X * pixelsPerUnit;
            var y = bodyPosition.Y * pixelsPerUnit;

            var angle = body.GetAngle();

            spriteBatch.Draw(
                texture,
                new Rectangle((int)x, (int)y, texWidth, texHeight),
                null,
                Microsoft.Xna.Framework.Color.White,
                angle,
                new Vector2(texWidth / 2, texHeight / 2),
                SpriteEffects.None,
                0);
        }

        private readonly Texture2D texture;
        private readonly Body body;
        private readonly int pixelsPerUnit;
    }
}
