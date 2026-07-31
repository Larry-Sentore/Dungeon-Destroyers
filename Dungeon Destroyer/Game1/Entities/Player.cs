using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Entities
{
    /// <summary>
    /// The player character: WASD movement, facing direction, animation, and health.
    /// </summary>
    public class Player
    {
        private enum AnimState { Idle, Run, Shoot }

        private readonly Texture2D texture;

        private AnimState animState = AnimState.Idle;
        private int animFrame;
        private float animTimer;
        private bool isMoving;

        public Vector2 Position;
        public bool FacingLeft { get; private set; }
        public int Health { get; private set; } = GameConstants.MaxHealth;

        /// <summary>Time left on the shoot animation. Overrides Run/Idle while above 0.</summary>
        public float ShootTimer { get; private set; }

        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
        }

        /// <summary>
        /// Using algebra to build the hitbox. It's inset to the character art, because
        /// the drawing only fills the middle of its 32x32 sprite cell.
        /// </summary>
        public Rectangle Bounds => new Rectangle(
            (int)(Position.X + GameConstants.WarriorArtOffsetX * GameConstants.WarriorScale),
            (int)(Position.Y + GameConstants.WarriorArtOffsetY * GameConstants.WarriorScale),
            (int)(GameConstants.WarriorArtWidth * GameConstants.WarriorScale),
            (int)(GameConstants.WarriorArtHeight * GameConstants.WarriorScale));

        /// <summary>Using algebra to find the centre, where bullets spawn from.</summary>
        public Vector2 Center => Position + new Vector2(
            GameConstants.WarriorFrameSize, GameConstants.WarriorFrameSize)
            * GameConstants.WarriorScale / 2f;

        /// <summary>Counts down the shoot animation timer.</summary>
        public void TickShootTimer(float deltaTime)
        {
            if (ShootTimer > 0f)
                ShootTimer -= deltaTime;
        }

        /// <summary>Restarts the shoot animation when a shot is fired.</summary>
        public void TriggerShootAnimation() => ShootTimer = GameConstants.ShootAnimDuration;

        /// <summary>Turns the sprite to match the direction it shot in.</summary>
        public void FaceLeft(bool faceLeft) => FacingLeft = faceLeft;

        /// <summary>Using algebra to work out health remaining, never below 0.</summary>
        public void TakeDamage(int amount)
        {
            Health = Math.Max(Health - amount, 0);
        }

        /// <summary>Using algebra to restore health, capped at the maximum.</summary>
        public void Heal(int amount)
        {
            Health = Math.Min(Health + amount, GameConstants.MaxHealth);
        }

        /// <summary>Reads WASD and moves the player using vectors.</summary>
        public void UpdateMovement(KeyboardState keyboard, float deltaTime)
        {
            // Builds a direction vector from the keys pressed.
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

            isMoving = direction != Vector2.Zero;
            if (!isMoving)
                return;

            // Normalising the vector keeps diagonal movement the same speed as straight.
            direction.Normalize();
            Position += direction * GameConstants.WarriorSpeed * deltaTime;

            // Only flip facing on left/right input, so moving up or down doesn't
            // reset the sprite to facing right.
            if (direction.X < 0) FacingLeft = true;
            else if (direction.X > 0) FacingLeft = false;
        }

        /// <summary>Chooses the animation (Shoot beats Run beats Idle) and advances frames.</summary>
        public void UpdateAnimation(float deltaTime)
        {
            AnimState newState =
                ShootTimer > 0f ? AnimState.Shoot :
                isMoving ? AnimState.Run :
                AnimState.Idle;

            // Restart from frame 0 whenever the animation changes.
            if (newState != animState)
            {
                animState = newState;
                animFrame = 0;
                animTimer = 0f;
            }

            int frameCount = animState switch
            {
                AnimState.Run => GameConstants.WarriorRunFrameCount,
                AnimState.Shoot => GameConstants.WarriorShootFrameCount,
                _ => 1, // Idle is a single frame.
            };

            if (frameCount <= 1)
                return;

            animTimer += deltaTime;
            if (animTimer < GameConstants.AnimFrameDuration)
                return;

            animTimer -= GameConstants.AnimFrameDuration;
            animFrame = (animFrame + 1) % frameCount;
        }

        /// <summary>Algebra: picks the frame out of the sprite sheet grid (column x row).</summary>
        private Rectangle GetSourceRectangle()
        {
            int row = animState switch
            {
                AnimState.Run => GameConstants.WarriorRunRow,
                AnimState.Shoot => GameConstants.WarriorShootRow,
                _ => GameConstants.WarriorIdleRow,
            };

            return new Rectangle(
                animFrame * GameConstants.WarriorFrameSize,
                row * GameConstants.WarriorFrameSize,
                GameConstants.WarriorFrameSize,
                GameConstants.WarriorFrameSize);
        }

        /// <summary>Rendering the player sprite, flipped when facing left.</summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Drawing from the centre so flipping mirrors in place instead of
            // shifting the sprite sideways.
            Vector2 origin = new Vector2(
                GameConstants.WarriorFrameSize / 2f, GameConstants.WarriorFrameSize / 2f);
            Vector2 drawPosition = Position + origin * GameConstants.WarriorScale;
            SpriteEffects effects = FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                drawPosition,
                GetSourceRectangle(),
                Color.White,
                0f,
                origin,
                GameConstants.WarriorScale,
                effects,
                0f);
        }
    }
}
