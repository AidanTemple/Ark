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

        public static Vector2 ClampToBounds(Vector2 position, Rectangle bounds, int halfWidth, int halfHeight)
        {
            float x = (int)MathHelper.Clamp(position.X, bounds.Left + halfWidth, bounds.Right - halfWidth);
            float y = (int)MathHelper.Clamp(position.Y, bounds.Top + halfHeight, bounds.Bottom - halfHeight);

            return new Vector2(x, y);
        }
    }
}
