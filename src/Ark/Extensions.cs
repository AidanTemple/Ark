#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    static class Extensions
    {
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        // No-op instead of throwing when content failed to load (see
        // ContentManager.TryLoad) -- every plain sprite Draw in the codebase
        // funnels through this, so guarding here covers them all without
        // repeating the null check at each call site.
        public static void DrawSafe(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle,
            Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (texture == null)
            {
                return;
            }

            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
