using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Game1.Entities;

namespace Game1.Systems
{
    /// <summary>
    /// Handles all damage: bullets hitting enemies, enemies touching the player, and
    /// potions being picked up. Runs after everything has moved, so hits are checked
    /// against final positions.
    /// </summary>
    public static class CombatSystem
    {
        /// <summary>Checks bullets against enemies and applies damage.</summary>
        /// <returns>Score earned from enemies killed this frame.</returns>
        public static int ResolveBulletHits(List<Bullet> bullets, List<Enemy> enemies)
        {
            int scoreEarned = 0;

            // Loops run backwards so removing an item doesn't skip the next one.
            for (int b = bullets.Count - 1; b >= 0; b--)
            {
                Rectangle bulletBounds = bullets[b].Bounds;

                for (int e = enemies.Count - 1; e >= 0; e--)
                {
                    Enemy enemy = enemies[e];

                    // Box collision: do the two rectangles overlap?
                    if (!bulletBounds.Intersects(enemy.Bounds))
                        continue;

                    enemy.TakeDamage(GameConstants.BulletDamage);
                    bullets.RemoveAt(b);

                    // Algebra: add the enemy's points to the score when it dies.
                    if (enemy.IsDead)
                    {
                        scoreEarned += enemy.ScoreValue;
                        enemies.RemoveAt(e);
                    }

                    // Bullet is used up, stop checking it.
                    break;
                }
            }

            return scoreEarned;
        }

        /// <summary>Damages the player when an enemy touches them. One hit per frame.</summary>
        public static void ResolveEnemyContact(Player player, List<Enemy> enemies)
        {
            Rectangle playerBounds = player.Bounds;

            foreach (Enemy enemy in enemies)
            {
                if (!playerBounds.Intersects(enemy.Bounds))
                    continue;

                if (!enemy.CanAttack)
                    continue;          // touching, but this one is on cooldown

                player.TakeDamage(enemy.ContactDamage);
                enemy.RegisterAttack();
                break;
            }
        }

        /// <summary>Picks up any potion the player is standing on and applies its effect.</summary>
        /// <returns>Score earned from potions collected this frame.</returns>
        public static int ResolvePotionPickups(Player player, List<Potion> potions)
        {
            int scoreEarned = 0;
            Rectangle playerBounds = player.Bounds;

            // Backwards again, and collected potions are removed from the list so it
            // doesn't grow forever while the spawner keeps adding to it.
            for (int i = potions.Count - 1; i >= 0; i--)
            {
                Potion potion = potions[i];
                if (!playerBounds.Intersects(potion.Bounds))
                    continue;

                player.Heal(potion.HealAmount);
                scoreEarned += potion.ScoreValue;
                potions.RemoveAt(i);
            }

            return scoreEarned;
        }
    }
}
