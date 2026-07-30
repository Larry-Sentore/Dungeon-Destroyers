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

        /// <summary>
        /// Builds a potion of one of the two kinds. Health potions restore hearts,
        /// yellow potions award score - a potion never does both.
        /// </summary>
        public static Potion Create(Vector2 position, bool isHealthPotion) => new Potion
        {
            Position = position,
            SourceFrame = isHealthPotion
                ? GameConstants.HealthPotionFrame
                : GameConstants.YellowPotionFrame,
            HealAmount = isHealthPotion ? GameConstants.HealthPotionHeal : 0,
            ScoreValue = isHealthPotion ? 0 : GameConstants.YellowPotionScore,
        };

        /// <summary>Screen-space bounds of this potion, used for pickup collision.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(SourceFrame.Width * GameConstants.PotionScale),
            (int)(SourceFrame.Height * GameConstants.PotionScale));

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
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
