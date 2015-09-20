#region Using statements
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Ark
{
    #region Particle Type

    public enum ParticleType
    {
        None,
        Enemy,
        Missile,
        Player,
    }

    #endregion

    #region Particle State

    public struct ParticleState
    {
        #region Private Members

        private static Random m_Random = new Random();

        #endregion

        #region Public Members

        public Vector2 Velocity;

        public ParticleType Type;

        public float LengthMultiplier;

        #endregion

        #region Initialisation

        public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier)
        {
            Velocity = velocity;
            Type = type;

            lengthMultiplier = 1f;
            LengthMultiplier = lengthMultiplier;
        }

        #endregion

        #region Update

        public static void Update(ParticleManager<ParticleState>.Particle particle)
        {
            var velocity = particle.State.Velocity;
            float speed = velocity.Length();

            Vector2.Add(ref particle.Position, ref velocity, out particle.Position);

            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;

            particle.Tint.A = (byte)(255 * alpha);

            if (particle.State.Type == ParticleType.Missile)
            {
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha); 
            }
            else
            {
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);
            }

            particle.Orientation = velocity.ToAngle();

            var position = particle.Position;

            if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < 0.00000000001f)
            {
                velocity = Vector2.Zero;
            }
            else if(particle.State.Type == ParticleType.Enemy)
            {
                velocity *= 0.94f;
            }
            else
            {
                velocity *= 0.96f + Math.Abs(position.X) % 0.04f;
            }

            particle.State.Velocity = velocity;
        }

        #endregion

        #region Helper Methods

        public static ParticleState GetRandom(float minVel, float maxVel)
        {
            var state = new ParticleState();

            state.Velocity = m_Random.NextVector2(minVel, maxVel);
            state.Type = ParticleType.None;
            state.LengthMultiplier = 1;

            return state;
        }

        #endregion
    }

    #endregion
}