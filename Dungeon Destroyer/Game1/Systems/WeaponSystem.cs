using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Entities;

namespace Game1.Systems
{
    /// <summary>
    /// Handles the player's shooting: aim input, fire rate, and moving the bullets.
    /// </summary>
    public class WeaponSystem
    {
        /// <summary>The live bullets. CombatSystem removes them as they hit enemies.</summary>
        public List<Bullet> Bullets { get; } = new List<Bullet>();

        private float fireCooldownTimer;

        /// <summary>Reads the arrow keys as an aim direction and fires that way.</summary>
        public void HandleInput(KeyboardState keyboard, Player player, float deltaTime)
        {
            fireCooldownTimer -= deltaTime;
            player.TickShootTimer(deltaTime);

            Vector2 aim = ReadAimDirection(keyboard);

            // Holding an arrow fires repeatedly; the cooldown sets the fire rate.
            if (aim == Vector2.Zero || fireCooldownTimer > 0f)
                return;

            Fire(player, aim);
            fireCooldownTimer = GameConstants.FireCooldown;
            player.TriggerShootAnimation();

            // Turn the sprite to face the way it shot.
            if (aim.X < 0) player.FaceLeft(true);
            else if (aim.X > 0) player.FaceLeft(false);
        }

        /// <summary>Builds an aim vector from the arrow keys.</summary>
        private static Vector2 ReadAimDirection(KeyboardState keyboard)
        {
            Vector2 aim = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.Up)) aim.Y -= 1;
            if (keyboard.IsKeyDown(Keys.Down)) aim.Y += 1;
            if (keyboard.IsKeyDown(Keys.Left)) aim.X -= 1;
            if (keyboard.IsKeyDown(Keys.Right)) aim.X += 1;

            // Opposite keys cancel out. Normalising stops diagonal shots being faster.
            if (aim != Vector2.Zero)
                aim.Normalize();

            return aim;
        }

        /// <summary>Vectors: bullet velocity = aim direction x bullet speed.</summary>
        private void Fire(Player player, Vector2 aimDirection)
        {
            Bullets.Add(new Bullet(player.Center, aimDirection * GameConstants.BulletSpeed));
        }

        /// <summary>Moves the bullets and deletes any that leave the screen.</summary>
        public void UpdateBullets(float deltaTime, Viewport viewport)
        {
            // Backwards so removing an item doesn't skip the next one.
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                Bullets[i].Update(deltaTime);

                // Checks all four edges, since bullets can now travel up and down.
                Vector2 position = Bullets[i].Position;
                if (position.X < 0 || position.X > viewport.Width ||
                    position.Y < 0 || position.Y > viewport.Height)
                {
                    Bullets.RemoveAt(i);
                }
            }
        }

        /// <summary>Rendering every live bullet.</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (Bullet bullet in Bullets)
                bullet.Draw(spriteBatch, pixel);
        }
    }
}
