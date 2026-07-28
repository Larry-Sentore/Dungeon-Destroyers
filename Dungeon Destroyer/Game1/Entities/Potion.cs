using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities
{
    /// <summary>
    /// A world pickup. Position and source frame live here so drawing and collision
    /// always agree, instead of repeating the same coordinates in two places.
    /// </summary>
    public class Potion
    {
        public Vector2 Position;
        public Rectangle SourceFrame;
        public int HealAmount;
        public int ScoreValue;
        public bool Collected;

        /// <summary>Screen-space bounds of this potion, used for pickup collision.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(SourceFrame.Width * GameConstants.PotionScale),
            (int)(SourceFrame.Height * GameConstants.PotionScale));

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (Collected)
                return;

            spriteBatch.Draw(
                texture,
                Position,
                SourceFrame,
                Color.White,
                0f,
                Vector2.Zero,
                GameConstants.PotionScale,
                SpriteEffects.None,
                0f);
        }
    }
}
