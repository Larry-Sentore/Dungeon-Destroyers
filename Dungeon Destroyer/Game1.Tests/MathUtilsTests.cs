using Microsoft.Xna.Framework;
using NUnit.Framework;
using Game1.Maths;

namespace Game1.Tests
{
    /// <summary> Tests for the maths helpers: distance, dot product, cross product and lerp. </summary>
    [TestFixture]
    public class MathUtilsTests
    {
        private const float Tolerance = 0.0001f;

        /// <summary>Distance: checks the straight-line length using a 3-4-5 triangle.</summary>
        [Test]
        public void Distance_ThreeFourFiveTriangle_ReturnsFive()
        {
            float result = MathUtils.Distance(new Vector2(0, 0), new Vector2(3, 4));

            Assert.That(result, Is.EqualTo(5f).Within(Tolerance));
        }

        /// <summary> Dot product: an enemy facing right must not be able to see a target that is behind it. </summary>
        [Test]
        public void IsWithinFieldOfView_TargetBehind_ReturnsFalse()
        {
            bool result = MathUtils.IsWithinFieldOfView(
                facing: new Vector2(1, 0), toTarget: new Vector2(-1, 0), fieldOfViewDegrees: 90f);

            Assert.That(result, Is.False);
        }

        /// <summary> Cross product: the sign decides which way an enemy turns. </summary>
        [Test]
        public void Cross_TargetAnticlockwise_ReturnsNegative()
        {
            float result = MathUtils.Cross(new Vector2(1, 0), new Vector2(0, -1));

            Assert.That(result, Is.LessThan(0f));
        }

        /// <summary> Lerp: an amount above 1 must be clamped, otherwise a long frame would make the health bar or a potion overshoot past its target. </summary>
        [Test]
        public void Lerp_AmountAboveOne_IsClampedToEndValue()
        {
            float result = MathUtils.Lerp(0f, 100f, 5f);

            Assert.That(result, Is.EqualTo(100f).Within(Tolerance));
        }
    }
}
