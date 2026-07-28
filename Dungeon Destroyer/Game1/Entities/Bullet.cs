using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities
{
    /// <summary>A single fired projectile. Travels in a straight line until it leaves the screen.</summary>
    public class Bullet
    {
        public Vector2 Position;
        public readonly Vector2 Velocity;

        public Bullet(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        /// <summary>Bullets are drawn centered on Position, so the hitbox is centered too.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - GameConstants.BulletSize / 2f),
            (int)(Position.Y - GameConstants.BulletSize / 2f),
            (int)GameConstants.BulletSize,
            (int)GameConstants.BulletSize);

        public void Update(float deltaTime) => Position += Velocity * deltaTime;

        /// <summary>Drawn as a scaled 1x1 pixel, so bullets need no dedicated sprite asset.</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            spriteBatch.Draw(
                pixel,
                Position,
                null,
                Color.Gold,
                0f,
                new Vector2(0.5f, 0.5f), // center of the 1x1 source texture
                GameConstants.BulletSize,
                SpriteEffects.None,
                0f);
        }
    }
}
