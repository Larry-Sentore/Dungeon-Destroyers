using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;
using Game1.Entities;

namespace Game1.Tests
{
    /// <summary> Tests for player health and movement. </summary>
    [TestFixture]
    public class PlayerTests
    {
        private Player player;

        [SetUp]
        public void SetUp()
        {
            // A fresh player for each test, so no test can affect another.
            player = new Player(null, new Vector2(100, 100));
        }

        /// <summary> Health calculation edge case: taking more damage than the player has left must stop at 0 </summary>
        [Test]
        public void TakeDamage_MoreThanRemainingHealth_FloorsAtZero()
        {
            player.TakeDamage(GameConstants.MaxHealth + 50);

            Assert.That(player.Health, Is.EqualTo(0));
        }

        /// <summary> Vectors: moving diagonally must cover the same distance as moving straight </summary>
        [Test]
        public void UpdateMovement_DiagonalIsNotFasterThanStraight()
        {
            Player straightMover = new Player(null, Vector2.Zero);
            Player diagonalMover = new Player(null, Vector2.Zero);

            straightMover.UpdateMovement(new KeyboardState(Keys.D), 1f);
            diagonalMover.UpdateMovement(new KeyboardState(Keys.D, Keys.S), 1f);

            Assert.That(
                diagonalMover.Position.Length(),
                Is.EqualTo(straightMover.Position.Length()).Within(0.001f));
        }
    }
}
