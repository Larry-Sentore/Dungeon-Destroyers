using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Maths;

namespace Game1.Entities
{
    /// <summary>A potion the player can pick up by walking over it.</summary>
    public class Potion
    {
        public Vector2 Position;
        public Rectangle SourceFrame;
        public int HealAmount;
        public int ScoreValue;

        /// <summary>
        /// Creates one of the two potion types. Health potions restore hearts,
        /// yellow potions give score - never both.
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

        /// <summary>Using algebra to size the pickup hitbox: frame size x scale.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(SourceFrame.Width * GameConstants.PotionScale),
            (int)(SourceFrame.Height * GameConstants.PotionScale));

        /// <summary>Using algebra to find the centre: position + half the size.</summary>
        public Vector2 Center => Position + HalfSize;

        /// <summary>Half the drawn size, used to line the potion up on the player.</summary>
        private Vector2 HalfSize => new Vector2(SourceFrame.Width, SourceFrame.Height)
            * GameConstants.PotionScale / 2f;

        /// <summary>
        /// Pulls the potion toward the player once they are close enough.
        /// </summary>
        public void Update(Vector2 playerCenter, float deltaTime)
        {
            // Distance: only potions inside the magnet radius get pulled in.
            if (MathUtils.Distance(Center, playerCenter) > GameConstants.PotionMagnetRadius)
                return;

            // Lerp: eases the potion toward the player instead of snapping across.
            Position = MathUtils.LerpPosition(
                Position,
                playerCenter - HalfSize,
                GameConstants.PotionMagnetSpeed * deltaTime);
        }

        /// <summary>Rendering the potion sprite.</summary>
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
