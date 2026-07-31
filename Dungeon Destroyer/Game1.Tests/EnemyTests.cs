using Microsoft.Xna.Framework;
using NUnit.Framework;
using Game1.Entities;

namespace Game1.Tests
{
    /// <summary>Tests for the stats that separate the two enemy types.</summary>
    [TestFixture]
    public class EnemyTests
    {
        /// <summary> Difficulty scaling: big enemies must be tougher, hit harder and move slower than small ones </summary>
        [Test]
        public void Create_BigEnemy_IsTougherAndSlowerThanSmall()
        {
            Enemy small = Enemy.Create(EnemyKind.Small, Vector2.Zero, Vector2.UnitX);
            Enemy big = Enemy.Create(EnemyKind.Big, Vector2.Zero, Vector2.UnitX);

            Assert.That(big.Health, Is.GreaterThan(small.Health));
            Assert.That(big.ContactDamage, Is.GreaterThan(small.ContactDamage));
            Assert.That(big.Speed, Is.LessThan(small.Speed));
        }
    }
}
