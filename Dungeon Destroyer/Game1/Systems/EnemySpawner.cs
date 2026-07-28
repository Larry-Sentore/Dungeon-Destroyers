using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Entities;

namespace Game1.Systems
{
    /// <summary>
    /// Adds a new enemy on a fixed timer, entering from a random screen edge.
    /// </summary>
    public class EnemySpawner
    {
        private readonly Random random = new Random();
        private float spawnTimer;

        public void Update(float deltaTime, Viewport viewport, List<Enemy> enemies)
        {
            spawnTimer -= deltaTime;
            if (spawnTimer > 0f)
                return;

            spawnTimer = GameConstants.EnemySpawnInterval;

            // Roughly 2 small enemies for every big one.
            EnemyKind kind = random.Next(3) == 0 ? EnemyKind.Big : EnemyKind.Small;
            enemies.Add(Enemy.Create(kind, GetRandomEdgePosition(viewport)));
        }

        /// <summary>Picks a random point just off one of the four screen edges.</summary>
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
