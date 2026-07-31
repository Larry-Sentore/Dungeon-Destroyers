using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities
{
    /// <summary>A bullet fired by the player. Moves in a straight line using vectors.</summary>
    public class Bullet
    {
        public Vector2 Position;
        public readonly Vector2 Velocity;

        public Bullet(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        /// <summary>Using algebra to build a hitbox centred on the bullet.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - GameConstants.BulletSize / 2f),
            (int)(Position.Y - GameConstants.BulletSize / 2f),
            (int)GameConstants.BulletSize,
            (int)GameConstants.BulletSize);

        /// <summary>Vector movement: new position = position + (velocity x time).</summary>
        public void Update(float deltaTime) => Position += Velocity * deltaTime;

        /// <summary>Rendering the bullet as a scaled 1x1 pixel.</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            spriteBatch.Draw(
                pixel,
                Position,
                null,
                Color.Gold,
                0f,
                new Vector2(0.5f, 0.5f), // centre of the 1x1 texture
                GameConstants.BulletSize,
                SpriteEffects.None,
                0f);
        }
    }
}
