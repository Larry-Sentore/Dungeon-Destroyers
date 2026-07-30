using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Entities;

namespace Game1.Systems
{
    /// <summary>
    /// Owns the player's projectiles: fire input and cooldown, bullet movement, and
    /// culling bullets that leave the screen.
    /// </summary>
    public class WeaponSystem
    {
        /// <summary>
        /// Exposed for <see cref="CombatSystem"/>, which removes bullets as they hit enemies.
        /// </summary>
        public List<Bullet> Bullets { get; } = new List<Bullet>();

        private float fireCooldownTimer;

        /// <summary>
        /// Reads the arrow keys as an aim direction and fires in that direction, subject
        /// to the cooldown. Also ticks the player's shoot animation timer, keeping weapon
        /// timing and animation timing in step.
        /// </summary>
        public void HandleInput(KeyboardState keyboard, Player player, float deltaTime)
        {
            fireCooldownTimer -= deltaTime;
            player.TickShootTimer(deltaTime);

            Vector2 aim = ReadAimDirection(keyboard);

            // Holding an arrow key fires repeatedly at the cooldown rate, so there's no
            // edge-trigger here - the cooldown alone paces the shots.
            if (aim == Vector2.Zero || fireCooldownTimer > 0f)
                return;

            Fire(player, aim);
            fireCooldownTimer = GameConstants.FireCooldown;
            player.TriggerShootAnimation();

            // Face the way we shot, so the sprite doesn't fire out of its own back.
            if (aim.X < 0) player.FaceLeft(true);
            else if (aim.X > 0) player.FaceLeft(false);
        }

        /// <summary>
        /// Turns the arrow keys into a direction. Opposite keys cancel out, and diagonals
        /// are normalized so they aren't faster than the cardinal directions.
        /// </summary>
        private static Vector2 ReadAimDirection(KeyboardState keyboard)
        {
            Vector2 aim = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.Up)) aim.Y -= 1;
            if (keyboard.IsKeyDown(Keys.Down)) aim.Y += 1;
            if (keyboard.IsKeyDown(Keys.Left)) aim.X -= 1;
            if (keyboard.IsKeyDown(Keys.Right)) aim.X += 1;

            if (aim != Vector2.Zero)
                aim.Normalize();

            return aim;
        }

        /// <summary>Spawns a bullet at the player's center, travelling along the aim direction.</summary>
        private void Fire(Player player, Vector2 aimDirection)
        {
            Bullets.Add(new Bullet(player.Center, aimDirection * GameConstants.BulletSpeed));
        }

        /// <summary>Moves active bullets and discards any that have left the screen.</summary>
        public void UpdateBullets(float deltaTime, Viewport viewport)
        {
            // Backwards so removing an item doesn't shift the indices still to be checked.
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Update(deltaTime);

                // Cull on both axes - bullets can now leave through the top and bottom
                // as well as the sides.
                Vector2 position = Bullets[i].Position;
                if (position.X < 0 || position.X > viewport.Width ||
                    position.Y < 0 || position.Y > viewport.Height)
                {
                    Bullets.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (Bullet bullet in Bullets)
                bullet.Draw(spriteBatch, pixel);
        }
    }
}
