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
        /// Handles the fire button and its cooldown. Also ticks the player's shoot
        /// animation timer, keeping weapon timing and animation timing in step.
        /// </summary>
        public void HandleInput(KeyboardState keyboard, KeyboardState previousKeyboard, Player player, float deltaTime)
        {
            fireCooldownTimer -= deltaTime;
            player.TickShootTimer(deltaTime);

            // Edge-triggered: only fires the instant the key is pressed, not every frame
            // it's held, so holding the key down doesn't machine-gun bullets.
            bool firePressed = keyboard.IsKeyDown(Keys.P) && !previousKeyboard.IsKeyDown(Keys.P);
            if (!firePressed || fireCooldownTimer > 0f)
                return;

            Fire(player);
            fireCooldownTimer = GameConstants.FireCooldown;
            player.TriggerShootAnimation();
        }

        /// <summary>Spawns a bullet at the player's center, travelling in the facing direction.</summary>
        private void Fire(Player player)
        {
            Vector2 velocity = new Vector2(
                player.FacingLeft ? -GameConstants.BulletSpeed : GameConstants.BulletSpeed, 0f);

            Bullets.Add(new Bullet(player.Center, velocity));
        }

        /// <summary>Moves active bullets and discards any that have left the screen.</summary>
        public void UpdateBullets(float deltaTime, Viewport viewport)
        {
            // Backwards so removing an item doesn't shift the indices still to be checked.
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Update(deltaTime);

                if (Bullets[i].Position.X < 0 || Bullets[i].Position.X > viewport.Width)
                    Bullets.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (Bullet bullet in Bullets)
                bullet.Draw(spriteBatch, pixel);
        }
    }
}
