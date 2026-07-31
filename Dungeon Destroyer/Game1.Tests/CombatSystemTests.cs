using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Game1.Entities;
using Game1.Systems;

namespace Game1.Tests
{
    /// <summary>
    /// Tests for the damage and scoring rules: bullets killing enemies, enemies
    /// touching the player, and potions being collected.
    /// </summary>
    [TestFixture]
    public class CombatSystemTests
    {
        private Player player;
        private List<Enemy> enemies;
        private List<Bullet> bullets;
        private List<Potion> potions;

        [SetUp]
        public void SetUp()
        {
            // Fresh lists every test so they stay independent and repeatable.
            player = new Player(null, new Vector2(100, 100));
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();
            potions = new List<Potion>();
        }

        /// <summary> Score calculation: the killing bullet removes the enemy and awards exactly that enemy's points. </summary>
        [Test]
        public void ResolveBulletHits_KillingBlow_RemovesEnemyAndAwardsScore()
        {
            Enemy enemy = Enemy.Create(EnemyKind.Small, new Vector2(200, 200), Vector2.UnitX);
            enemies.Add(enemy);

            // Small enemies have 2 health
            enemy.TakeDamage(GameConstants.SmallEnemyHealth - GameConstants.BulletDamage);
            bullets.Add(new Bullet(enemy.Center, Vector2.Zero));

            int score = CombatSystem.ResolveBulletHits(bullets, enemies);

            Assert.That(enemies, Is.Empty);
            Assert.That(score, Is.EqualTo(GameConstants.SmallEnemyScore));
        }

        /// <summary> Cooldown edge case: an enemy that has just attacked deals no damage while still touching the player. </summary>
        [Test]
        public void ResolveEnemyContact_EnemyOnCooldown_DealsNoDamage()
        {
            Enemy enemy = Enemy.Create(EnemyKind.Small, new Vector2(player.Bounds.X, player.Bounds.Y), Vector2.UnitX);
            enemies.Add(enemy);
            enemy.RegisterAttack();

            CombatSystem.ResolveEnemyContact(player, enemies);

            Assert.That(player.Health, Is.EqualTo(GameConstants.MaxHealth));
        }

        /// <summary>
        /// Power-up effect: walking over a health potion restores health and takes the potion out of the world so it cannot be collected twice.
        /// </summary>
        [Test]
        public void ResolvePotionPickups_HealthPotion_HealsPlayerAndIsRemoved()
        {
            player.TakeDamage(5);
            potions.Add(Potion.Create(new Vector2(player.Bounds.X, player.Bounds.Y), isHealthPotion: true));

            CombatSystem.ResolvePotionPickups(player, potions);

            Assert.That(player.Health, Is.EqualTo(GameConstants.MaxHealth - 5 + GameConstants.HealthPotionHeal));
            Assert.That(potions, Is.Empty);
        }
    }
}
