using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Maths;

namespace Game1.Entities
{
    /// <summary>
    /// An enemy that hunts the player. It steers using its facing direction, spots the
    /// player with a field of view check, and damages them on contact.
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

        /// <summary>The way this enemy is currently pointing. It walks along this.</summary>
        public Vector2 Facing = Vector2.UnitX;

        /// <summary>True while the player is inside this enemy's field of view.</summary>
        public bool IsAlerted { get; private set; }

        private int animFrame;
        private float animTimer;
        private float attackCooldown;
        private float damageFlash;

        /// <summary>True once this enemy's attack cooldown has run out.</summary>
        public bool CanAttack => attackCooldown <= 0f;

        /// <summary>Starts the cooldown after this enemy lands a hit.</summary>
        public void RegisterAttack() => attackCooldown = GameConstants.EnemyAttackCooldown;

        /// <summary>Creates an enemy with the stats for its type (small or big).</summary>
        public static Enemy Create(EnemyKind kind, Vector2 position, Vector2 facing)
        {
            bool isBig = kind == EnemyKind.Big;

            return new Enemy
            {
                Position = position,
                Facing = facing,
                Kind = kind,
                Health = isBig ? GameConstants.BigEnemyHealth : GameConstants.SmallEnemyHealth,
                ContactDamage = isBig ? GameConstants.BigEnemyDamage : GameConstants.SmallEnemyDamage,
                Speed = isBig ? GameConstants.BigEnemySpeed : GameConstants.SmallEnemySpeed,
                Scale = isBig ? GameConstants.BigEnemyScale : GameConstants.SmallEnemyScale,
                ScoreValue = isBig ? GameConstants.BigEnemyScore : GameConstants.SmallEnemyScore,
            };
        }

        /// <summary>Using algebra to size the hitbox: box size = frame size x scale.</summary>
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(GameConstants.PumpkinFrameWidth * Scale),
            (int)(GameConstants.PumpkinFrameHeight * Scale));

        /// <summary>Using algebra to find the centre point: position + half the size.</summary>
        public Vector2 Center => Position + new Vector2(
            GameConstants.PumpkinFrameWidth * Scale / 2f,
            GameConstants.PumpkinFrameHeight * Scale / 2f);

        public bool IsDead => Health <= 0;

        /// <summary>Using algebra to subtract bullet damage from health.</summary>
        public void TakeDamage(int amount)
        {
            Health -= amount;

            // Starts the damage tint at full red, which then lerps back to normal.
            damageFlash = 1f;
        }

        /// <summary>Steers the enemy toward the player and runs its walk animation.</summary>
        public void Update(Vector2 target, float deltaTime)
        {
            // Distance: how far away the player is.
            float distanceToPlayer = MathUtils.Distance(Center, target);

            // Direction: unit vector pointing at the player.
            Vector2 toPlayer = MathUtils.Direction(Center, target);

            // Dot product: the enemy only spots the player if they are close enough
            // AND inside the cone it is facing.
            IsAlerted = distanceToPlayer <= GameConstants.EnemyDetectionRadius
                && MathUtils.IsWithinFieldOfView(Facing, toPlayer, GameConstants.EnemyFieldOfViewDegrees);

            // Cross product: works out whether to turn left or right, so the enemy
            // rotates round gradually instead of snapping to face the player.
            Facing = MathUtils.TurnToward(Facing, toPlayer, GameConstants.EnemyTurnSpeed * deltaTime);

            // Vectors: moves forward along its facing. It charges faster once alerted.
            float currentSpeed = IsAlerted ? Speed * GameConstants.EnemyChargeMultiplier : Speed;
            Position += Facing * currentSpeed * deltaTime;

            // Lerp: fades the red damage tint back down toward 0.
            damageFlash = MathUtils.Lerp(damageFlash, 0f, GameConstants.EnemyFlashFadeSpeed * deltaTime);

            // Cycles through the animation frames on a timer.
            animTimer += deltaTime;
            if (animTimer >= GameConstants.EnemyAnimFrameDuration)
            {
                animTimer -= GameConstants.EnemyAnimFrameDuration;
                animFrame = (animFrame + 1) % GameConstants.PumpkinFrameCount;
            }

            if (attackCooldown > 0f)
                attackCooldown -= deltaTime;
        }

        /// <summary>Rendering the enemy's current animation frame.</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            // Algebra: picks the frame out of the sprite sheet (frame number x width).
            Rectangle source = new Rectangle(
                animFrame * GameConstants.PumpkinFrameWidth,
                0,
                GameConstants.PumpkinFrameWidth,
                GameConstants.PumpkinFrameHeight);

            // Lerp: blends the tint between white and red based on the damage flash.
            Color tint = MathUtils.LerpColor(Color.White, Color.Red, damageFlash);

            // Flips the sprite so it looks the way it is walking.
            SpriteEffects effects = Facing.X < 0
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                Position,
                source,
                tint,
                0f,
                Vector2.Zero,
                Scale,
                effects,
                0f);
        }
    }
}
