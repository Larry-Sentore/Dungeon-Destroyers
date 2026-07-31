using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Entities;
using Game1.Maths;

namespace Game1.Systems
{
    /// <summary>Spawns a new enemy on a timer, entering from a random screen edge.</summary>
    public class EnemySpawner
    {
        private readonly Random random = new Random();
        private float spawnTimer;

        public void Update(float deltaTime, Viewport viewport, List<Enemy> enemies)
        {
            // Counts down and spawns when the timer hits zero.
            spawnTimer -= deltaTime;
            if (spawnTimer > 0f)
                return;

            spawnTimer = GameConstants.EnemySpawnInterval;

            // Roughly 2 small enemies for every big one.
            EnemyKind kind = random.Next(3) == 0 ? EnemyKind.Big : EnemyKind.Small;

            // Direction: starts the enemy facing the middle of the screen, so it walks
            // into view instead of wandering off before it turns.
            Vector2 spawnPosition = GetRandomEdgePosition(viewport);
            Vector2 screenCentre = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
            Vector2 startFacing = MathUtils.Direction(spawnPosition, screenCentre);

            enemies.Add(Enemy.Create(kind, spawnPosition, startFacing));
        }

        /// <summary>
        /// Picks a random point just off one of the four screen edges. Enemies walk
        /// on-screen by themselves, so spawning outside the view is fine.
        /// </summary>
        private Vector2 GetRandomEdgePosition(Viewport viewport)
        {
            return random.Next(4) switch
            {
                0 => new Vector2(random.Next(viewport.Width), -100),                  // top
                1 => new Vector2(random.Next(viewport.Width), viewport.Height + 20),  // bottom
                2 => new Vector2(-100, random.Next(viewport.Height)),                 // left
                _ => new Vector2(viewport.Width + 20, random.Next(viewport.Height)),  // right
            };
        }
    }
}
