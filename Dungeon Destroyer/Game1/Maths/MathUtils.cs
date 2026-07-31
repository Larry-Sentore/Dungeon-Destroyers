using System;
using Microsoft.Xna.Framework;

namespace Game1.Maths
{
    /// <summary>
    /// All the maths the game uses, in one place: distance, direction, dot product, cross product and linear interpolation (lerp).
    /// </summary>
    public static class MathUtils
    {
        // ------------------------------------------------------------------
        // Distance and direction
        // ------------------------------------------------------------------

        /// <summary>Distance: the straight-line length between two points.</summary>
        public static float Distance(Vector2 a, Vector2 b) => Vector2.Distance(a, b);

        /// <summary>
        /// Direction: a unit vector (length 1) pointing from one point to another.
        /// Subtracting the points gives the direction, normalising removes the distance.
        /// </summary>
        public static Vector2 Direction(Vector2 from, Vector2 to)
        {
            Vector2 difference = to - from;
            return difference == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(difference);
        }

        // ------------------------------------------------------------------
        // Dot product
        // ------------------------------------------------------------------

        /// <summary>
        /// Dot product: (a.X * b.X) + (a.Y * b.Y). For unit vectors this equals the
        /// cosine of the angle between them, so 1 means same direction, 0 means 90
        /// degrees apart, and -1 means opposite directions.
        /// </summary>
        public static float Dot(Vector2 a, Vector2 b) => (a.X * b.X) + (a.Y * b.Y);

        /// <summary>
        /// Uses the dot product to check whether a target sits inside a cone in front
        /// of something. This is the enemy field of view test.
        /// </summary>
        public static bool IsWithinFieldOfView(Vector2 facing, Vector2 toTarget, float fieldOfViewDegrees)
        {
            if (facing == Vector2.Zero || toTarget == Vector2.Zero)
                return false;

            // Half the cone angle, turned into a cosine so it can be compared directly
            // against the dot product.
            float cosineLimit = MathF.Cos(MathHelper.ToRadians(fieldOfViewDegrees / 2f));

            return Dot(Vector2.Normalize(facing), Vector2.Normalize(toTarget)) >= cosineLimit;
        }

        // ------------------------------------------------------------------
        // Cross product
        // ------------------------------------------------------------------

        /// <summary>
        /// Cross product in 2D: (a.X * b.Y) - (a.Y * b.X). It returns a single number
        /// whose sign tells you which side b is on - positive means b is clockwise from
        /// a, negative means anticlockwise, zero means they line up.
        /// </summary>
        public static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);

        /// <summary>Rotates a vector by an angle in radians.</summary>
        public static Vector2 Rotate(Vector2 vector, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            return new Vector2(
                (vector.X * cos) - (vector.Y * sin),
                (vector.X * sin) + (vector.Y * cos));
        }

        /// <summary>
        /// Turns a facing direction toward a target by at most maxTurn radians. The
        /// cross product decides whether to turn left or right, which is how the
        /// enemies orientate themselves instead of snapping round instantly.
        /// </summary>
        public static Vector2 TurnToward(Vector2 facing, Vector2 target, float maxTurn)
        {
            if (facing == Vector2.Zero || target == Vector2.Zero)
                return facing;

            Vector2 from = Vector2.Normalize(facing);
            Vector2 to = Vector2.Normalize(target);

            // Dot product gives the angle still to turn through.
            float angle = MathF.Acos(MathHelper.Clamp(Dot(from, to), -1f, 1f));

            // Close enough this frame, so finish the turn exactly.
            if (angle <= maxTurn)
                return to;

            // Cross product picks the turn direction: +1 clockwise, -1 anticlockwise.
            float turnSide = MathF.Sign(Cross(from, to));

            // Exactly opposite directions give a cross product of 0, which would mean
            // no turn at all. Pick a side so the turn still happens.
            if (turnSide == 0f)
                turnSide = 1f;

            return Rotate(from, turnSide * maxTurn);
        }

        // ------------------------------------------------------------------
        // Linear interpolation (lerp)
        // ------------------------------------------------------------------

        /// <summary>
        /// Lerp: blends smoothly between two numbers. t = 0 gives a, t = 1 gives b,
        /// and 0.5 gives the halfway point. Formula is a + ((b - a) * t).
        /// </summary>
        public static float Lerp(float a, float b, float t) => a + ((b - a) * Clamp01(t));

        /// <summary>Lerp between two colours, used for fading a tint in and out.</summary>
        public static Color LerpColor(Color a, Color b, float t) => Color.Lerp(a, b, Clamp01(t));

        /// <summary>Lerp between two positions, used for easing something into place.</summary>
        public static Vector2 LerpPosition(Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, Clamp01(t));

        /// <summary>Keeps a lerp amount inside the 0 to 1 range.</summary>
        private static float Clamp01(float t) => MathHelper.Clamp(t, 0f, 1f);
    }
}
