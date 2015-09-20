#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    public class ParticleManager<T>
    {
        #region Particle Class

        public class Particle
        {
            #region Public Members

            public Texture2D Texture;

            public Vector2 Position;
            public Vector2 Scale;

            public float Orientation;
            public float Duration;
            public float PercentLife;

            public Color Tint;

            public T State;

            #endregion

            #region Initialisation

            public Particle()
            {
                Scale = Vector2.One;

                PercentLife = 1f;
            }

            #endregion
        }

        #endregion

        #region Circular Particle Array Class

        private class CircularParticleArray
        {
            #region Private Members

            private int m_Start;

            private Particle[] m_List;

            #endregion

            #region Properties

            public int Start
            {
                get { return m_Start; }
                set { m_Start = value % m_List.Length; }
            }

            public int Count
            {
                get;
                set;
            }

            public int Capacity
            {
                get { return m_List.Length; }
            }

            public Particle this[int i]
            {
                get { return m_List[(m_Start + i) % m_List.Length]; }
                set { m_List[(m_Start + i) % m_List.Length] = value; }
            }

            #endregion

            #region Initialisation

            public CircularParticleArray(int capacity)
            {
                m_List = new Particle[capacity];
            }

            #endregion
        }

        #endregion

        #region Private Members

        private Action<Particle> m_UpdateParticle;

        private CircularParticleArray m_ParticleList;

        #endregion

        #region Initialisation

        public ParticleManager(int capacity, Action<Particle> update)
        {
            this.m_UpdateParticle = update;
            m_ParticleList = new CircularParticleArray(capacity);

            for (int i = 0; i < capacity; i++)
            {
                m_ParticleList[i] = new Particle();
            }
        }

        #endregion

        #region Update

        public void Update()
        {
            int removalCount = 0;

            for (int i = 0; i < m_ParticleList.Count; i++)
            {
                var particle = m_ParticleList[i];

                m_UpdateParticle(particle);

                particle.PercentLife -= 1f / particle.Duration;

                Swap(m_ParticleList, i - removalCount, i);

                if (particle.PercentLife < 0)
                {
                    removalCount++;
                }
            }

            m_ParticleList.Count -= removalCount;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < m_ParticleList.Count; i++)
            {
                var particle = m_ParticleList[i];

                Vector2 origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);

                spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation,
                    origin, particle.Scale, 0, 0);
            }
        }

        #endregion

        #region Helper Methods

        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state)
        {
            float theta = 0;

            CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
        }

        public void CreateParticle(Texture2D texture, Vector2 position, Color tint,
            float duration, Vector2 scale, T state, float theta)
        {
            theta = 0;

            Particle particle;

            if (m_ParticleList.Count == m_ParticleList.Capacity)
            {
                particle = m_ParticleList[0];
                m_ParticleList.Start++;
            }
            else
            {
                particle = m_ParticleList[m_ParticleList.Count];
                m_ParticleList.Count++;
            }

            particle.Texture = texture;
            particle.Position = position;
            particle.Tint = tint;
            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;
        }

        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        #endregion
    }
}