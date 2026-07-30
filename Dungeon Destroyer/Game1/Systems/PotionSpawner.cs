using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Entities;


namespace Game1.Systems
{
    /// <summary>
    /// Drops a new potion on a fixed timer at a random reachable spot on screen.
    /// </summary>
    public class PotionSpawner
    {
        private readonly Random random = new Random();
        private float spawnTimer;

        public void Update(float deltaTime, Viewport viewport, List<Potion> potions)
        {
            spawnTimer -= deltaTime;
            if (spawnTimer > 0f)
                return;

            spawnTimer = GameConstants.PotionSpawnInterval;

            // Even split between the two kinds.
            bool isHealthPotion = random.Next(2) == 0;
            potions.Add(Potion.Create(GetRandomPosition(viewport), isHealthPotion));
        }

        /// <summary>
        /// Picks a random point inside the play area. Unlike enemies, potions never
        /// move, so spawning one off-screen would put it permanently out of reach.
        /// </summary>
        private Vector2 GetRandomPosition(Viewport viewport)
        {
            return new Vector2(
                random.Next(
                    GameConstants.PotionSpawnMargin,
                    viewport.Width - GameConstants.PotionSpawnMargin),
                random.Next(
                    GameConstants.PotionSpawnTopMargin,
                    viewport.Height - GameConstants.PotionSpawnMargin));
        }
    }
}