using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Entities
{
    /// <summary>
    /// A chasing enemy. Health, contact damage, speed and size are all set from
    /// <see cref="EnemyKind"/> at spawn time by <see cref="Create"/>, so the rest of
    /// the game reads these fields instead of re-checking which kind it is.
    /// </summary>
    public class Enemy
    {
        public Vector2 Position;
        public EnemyKind Kind;
        public int Health;
        public int ContactDamage;
        public float Speed;
        public float Scale;
        public int ScoreValue;


        private int animFrame;
        private float animTimer;
        private float attackCooldown;

        public bool CanAttack => attackCooldown <= 0f;
        public void RegisterAttack() => attackCooldown = GameConstants.EnemyAttackCooldown;

        /// <summary>Builds an enemy of the given kind with that kind's stats.</summary>
        public static Enemy Create(EnemyKind kind, Vector2 position)
        {
            bool isBig = kind == EnemyKind.Big;

            return new Enemy
            {
                Position = position,
                Kind = kind,
                Health = isBig ? GameConstants.BigEnemyHealth : GameConstants.SmallEnemyHealth,
                ContactDamage = isBig ? GameConstants.BigEnemyDamage : GameConstants.SmallEnemyDamage,
                Speed = isBig ? GameConstants.BigEnemySpeed : GameConstants.SmallEnemySpeed,
                Scale = isBig ? GameConstants.BigEnemyScale : GameConstants.SmallEnemyScale,
                ScoreValue = isBig ? GameConstants.BigEnemyScore : GameConstants.SmallEnemyScore,
            };
        }

        /// <summary>Screen-space bounds, used for both bullet hits and player contact.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(GameConstants.PumpkinFrameWidth * Scale),
            (int)(GameConstants.PumpkinFrameHeight * Scale));

        /// <summary>Center point, used to steer the enemy toward the player.</summary>
        public Vector2 Center => Position + new Vector2(
            GameConstants.PumpkinFrameWidth * Scale / 2f,
            GameConstants.PumpkinFrameHeight * Scale / 2f);

        public bool IsDead => Health <= 0;

        /// <summary>Applies bullet damage.</summary>
        public void TakeDamage(int amount)
        {
            Health -= amount;
        }

        /// <summary>Walks straight at the target and advances the walk animation.</summary>
        public void Update(Vector2 target, float deltaTime)
        {
            // Normalizing gives a pure direction, so speed stays constant 

            Vector2 toTarget = target - Center;
            if (toTarget != Vector2.Zero)
            {
                toTarget.Normalize();
                Position += toTarget * Speed * deltaTime;
            }

            animTimer += deltaTime;
            if (animTimer >= GameConstants.EnemyAnimFrameDuration)
            {
                animTimer -= GameConstants.EnemyAnimFrameDuration;
                animFrame = (animFrame + 1) % GameConstants.PumpkinFrameCount;
            }

            if (attackCooldown > 0f)
                attackCooldown -= deltaTime;
        }

        /// <summary>Draws the enemy's current walk-animation frame.</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Rectangle source = new Rectangle(
                animFrame * GameConstants.PumpkinFrameWidth,
                0,
                GameConstants.PumpkinFrameWidth,
                GameConstants.PumpkinFrameHeight);

            spriteBatch.Draw(
                texture,
                Position,
                source,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}
