using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Entities
{
    /// <summary>
    /// The player-controlled warrior: WASD movement, facing, the Idle/Run/Shoot
    /// animation state machine, health, and the post-hit invulnerability window.
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

        /// <summary>Seconds remaining of the shoot animation. Overrides Run/Idle while &gt; 0.</summary>
        public float ShootTimer { get; private set; }

        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            Position = startPosition;
        }

        /// <summary>
        /// Screen-space hitbox, inset to the character art rather than the full scaled
        /// frame so collisions line up with what the player can actually see.
        /// </summary>
        public Rectangle Bounds => new Rectangle(
            (int)(Position.X + GameConstants.WarriorArtOffsetX * GameConstants.WarriorScale),
            (int)(Position.Y + GameConstants.WarriorArtOffsetY * GameConstants.WarriorScale),
            (int)(GameConstants.WarriorArtWidth * GameConstants.WarriorScale),
            (int)(GameConstants.WarriorArtHeight * GameConstants.WarriorScale));

        /// <summary>Center of the full sprite frame - where bullets spawn from.</summary>
        public Vector2 Center => Position + new Vector2(
            GameConstants.WarriorFrameSize, GameConstants.WarriorFrameSize)
            * GameConstants.WarriorScale / 2f;

        /// <summary>Counts down the shoot animation.</summary>
        public void TickShootTimer(float deltaTime)
        {
            if (ShootTimer > 0f)
                ShootTimer -= deltaTime;
        }

        /// <summary>Restarts the shoot animation, called when a shot is actually fired.</summary>
        public void TriggerShootAnimation() => ShootTimer = GameConstants.ShootAnimDuration;

        /// <summary>Applies damage, floored at 0.</summary>
        public void TakeDamage(int amount)
        {
            Health = Math.Max(Health - amount, 0);
        }

        /// <summary>Restores health, capped at <see cref="GameConstants.MaxHealth"/>.</summary>
        public void Heal(int amount)
        {
            Health = Math.Min(Health + amount, GameConstants.MaxHealth);
        }

        /// <summary>Reads WASD input, moves the warrior, and tracks left/right facing.</summary>
        public void UpdateMovement(KeyboardState keyboard, float deltaTime)
        {
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

            isMoving = direction != Vector2.Zero;
            if (!isMoving)
                return;

            // Normalize so diagonal movement isn't faster than cardinal movement.
            direction.Normalize();
            Position += direction * GameConstants.WarriorSpeed * deltaTime;

            // Only flip facing on horizontal input; ignore vertical-only movement so
            // the sprite doesn't snap back to "facing right" while moving up/down.
            if (direction.X < 0) FacingLeft = true;
            else if (direction.X > 0) FacingLeft = false;
        }

        /// <summary>
        /// Picks the current animation state (Shoot takes priority over Run/Idle) and
        /// advances the frame timer.
        /// </summary>
        public void UpdateAnimation(float deltaTime)
        {
            AnimState newState =
                ShootTimer > 0f ? AnimState.Shoot :
                isMoving ? AnimState.Run :
                AnimState.Idle;

            // Restart the frame counter whenever the state changes so, e.g., every shot
            // plays its animation from frame 0 instead of resuming mid-cycle.
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
                _ => 1, // Idle only has a single frame in the sheet.
            };

            if (frameCount <= 1)
                return;

            animTimer += deltaTime;
            if (animTimer < GameConstants.AnimFrameDuration)
                return;

            animTimer -= GameConstants.AnimFrameDuration;
            animFrame = (animFrame + 1) % frameCount;
        }

        /// <summary>Maps the current animation state/frame to a rectangle within the sprite sheet.</summary>
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

        /// <summary>Draws the warrior using its current animation frame, flipped when facing left.</summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Origin at the frame's center so flipping mirrors the sprite in place
            // instead of shifting it sideways by a frame width.
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
