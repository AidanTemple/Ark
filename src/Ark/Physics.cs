#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    // Pure functions over primitives (Rectangle/Vector2/float) -- no
    // dependency on any gameplay type, so anything with a position or a
    // bounding rect can use this without Physics needing to know it exists.
    public static class Physics
    {
        public static bool Overlaps(Rectangle a, Rectangle b)
        {
            return a.Intersects(b);
        }

        public static bool IsOutOfBounds(Vector2 position, Rectangle bounds)
        {
            return !bounds.Contains((int)position.X, (int)position.Y);
        }

        public static bool IsWithinRadius(Vector2 a, Vector2 b, float radius)
        {
            return Vector2.Distance(a, b) <= radius;
        }

        // Below a LengthSquared of 0.01, the direction vector is used
        // un-normalized (a damped nudge) instead of a full-speed step --
        // avoids jitter/overshoot right at the pull's center.
        public static Vector2 CalculatePullStep(Vector2 position, Vector2 origin, float speed)
        {
            Vector2 toOrigin = origin - position;

            if (toOrigin.LengthSquared() > 0.01f)
            {
                toOrigin.Normalize();
            }

            return toOrigin * speed;
        }

        // bounds.Width/bounds.Height (not bounds.Right/bounds.Bottom) as the
        // upper edge is intentional -- matches the original Player clamp
        // formula, which relies on bounds.X == 0 / bounds.Y == 0.
        public static Vector2 ClampToBounds(Vector2 position, Rectangle bounds, int halfWidth, int halfHeight)
        {
            float x = (int)MathHelper.Clamp(position.X, bounds.X + halfWidth, bounds.Width - halfWidth);
            float y = (int)MathHelper.Clamp(position.Y, bounds.Y + halfHeight, bounds.Height - halfHeight);

            return new Vector2(x, y);
        }
    }
}
