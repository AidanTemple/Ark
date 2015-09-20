#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    static class Extensions
    {
        public static Random Random = new Random();

        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static Vector2 NextVector2(this Random random, float minLength, float maxLength)
        {
            double theta = random.NextDouble() * 2 * Math.PI;
            float length = random.NextFloat(minLength, maxLength);

            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }

        public static Vector2 CenterString(SpriteFont font, String text, int width, int height)
        {
            Vector2 length = font.MeasureString(text);
            Vector2 position = new Vector2((width / 2) - (length.X / 2), (height / 2) - (length.Y / 2));

            return position;
        }
    }
}
