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

        // For entities that legitimately live outside the tight viewport for
        // a while (e.g. an asteroid spawned just off-screen, or one not yet
        // drifted clear after being destroyed) -- inflates the bounds by
        // margin on every side before testing.
        public static bool IsOutOfBounds(Vector2 position, Rectangle bounds, int margin)
        {
            Rectangle inflated = bounds;
            inflated.Inflate(margin, margin);

            return !inflated.Contains((int)position.X, (int)position.Y);
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

        public static Vector2 ClampToBounds(Vector2 position, Rectangle bounds, int halfWidth, int halfHeight)
        {
            float x = (int)MathHelper.Clamp(position.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
            float y = (int)MathHelper.Clamp(position.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);

            return new Vector2(x, y);
        }

        // Standard closest-point-on-a-ray projection: how far along
        // origin + velocity*t the moving point comes closest to target.
        // Returned in whatever units velocity already is (this codebase's
        // velocities are per-frame deltas, so this comes out in frames).
        // Clamped to >= 0 so a target the ray has already passed (or a
        // stationary velocity) doesn't report a phantom future approach.
        public static float TimeToClosestApproach(Vector2 origin, Vector2 velocity, Vector2 target)
        {
            float speedSquared = velocity.LengthSquared();

            if (speedSquared < 0.0001f)
            {
                return 0f;
            }

            float t = Vector2.Dot(target - origin, velocity) / speedSquared;

            return MathHelper.Max(t, 0f);
        }
    }
}
