using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Game1.Entities;

namespace Game1.Systems
{
    /// <summary>
    /// All damage resolution: bullets hitting enemies, enemies touching the player,
    /// player walking over potions. Runs after everything has moved for the
    /// frame, so hits are tested against final positions rather than stale ones.
    /// </summary>
    public static class CombatSystem
    {


        /// <returns>Score earned from enemies killed this frame.</returns>
        public static int ResolveBulletHits(List<Bullet> bullets, List<Enemy> enemies)
        {
            int scoreEarned = 0;

            // Both loops run backwards so removing an item doesn't shift the indices
            // of the entries still to be checked.
            for (int b = bullets.Count - 1; b >= 0; b--)
            {
                Rectangle bulletBounds = bullets[b].Bounds;

                for (int e = enemies.Count - 1; e >= 0; e--)
                {
                    Enemy enemy = enemies[e];
                    if (!bulletBounds.Intersects(enemy.Bounds))
                        continue;

                    enemy.TakeDamage(GameConstants.BulletDamage);
                    bullets.RemoveAt(b);

                    if (enemy.IsDead)
                    {
                        scoreEarned += enemy.ScoreValue;
                        enemies.RemoveAt(e);
                    }

                    // This bullet is spent - stop checking it against other enemies.
                    break;
                }
            }

            return scoreEarned;
        }

        /// <summary>
        /// Damages the player when an enemy touches them. At most one hit lands per
        /// frame, no matter how many enemies are overlapping.
        /// </summary>
        public static void ResolveEnemyContact(Player player, List<Enemy> enemies)
        {
            Rectangle playerBounds = player.Bounds;

            foreach (Enemy enemy in enemies)
            {
                if (!playerBounds.Intersects(enemy.Bounds))
                    continue;

                if (!enemy.CanAttack)
                    continue;          // still touching, but this one just hit you

                player.TakeDamage(enemy.ContactDamage);
                enemy.RegisterAttack();
                break;
            }
        }

        /// <summary>
        /// Collects any potion the player is overlapping: applies its effect, then marks
        /// it collected so it stops being drawn and can't be picked up twice.
        /// </summary>
        /// <returns>Score earned from potions collected this frame.</returns>
        public static int ResolvePotionPickups(Player player, List<Potion> potions)
        {
            int scoreEarned = 0;
            Rectangle playerBounds = player.Bounds;

            // Backwards so removing an item doesn't shift the indices still to be
            // checked. Collected potions are removed rather than just flagged, so the
            // list doesn't grow without bound while the spawner keeps adding to it.
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
