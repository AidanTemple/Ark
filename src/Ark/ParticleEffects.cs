#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    public static class ParticleEffects
    {
        public static void SpawnBurst(ParticleManager<ParticleState> particles, int width, int height,
            Vector2 position, int particleCount, Color colorA, Color colorB, int duration, ParticleType type)
        {
            Vector2 pos = new Vector2(position.X + width / 2, position.Y + height / 2);

            for (int i = 0; i < particleCount; i++)
            {
                float speed = 18f * (1f - 1 / Extensions.Random.NextFloat(1f, 10f));

                var state = new ParticleState()
                {
                    Velocity = Extensions.Random.NextVector2(speed, speed),
                    Type = type,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(colorA, colorB, Extensions.Random.NextFloat(0, 1));
                particles.CreateParticle(ContentManager.LineParticle, pos, color, duration, 1f, state);
            }
        }
    }
}
