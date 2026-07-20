#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Ark
{
    // Independent of WaveManager/Wave on purpose -- asteroids are a
    // continuous background hazard on their own timer, not tied to enemy
    // wave progression.
    class AsteroidManager
    {
        #region Private Members

        private GraphicsDevice m_GraphicsDevice;
        private Rectangle m_ViewportRect;

        private float m_SpawnTimer;

        private List<Asteroid> m_Asteroids = new List<Asteroid>();

        #endregion

        #region Properties

        public List<Asteroid> Asteroids
        {
            get { return m_Asteroids; }
        }

        #endregion

        #region Initialisation

        public AsteroidManager(GraphicsDevice graphicsDevice)
        {
            m_GraphicsDevice = graphicsDevice;

            Viewport viewport = graphicsDevice.Viewport;
            m_ViewportRect = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            m_SpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_SpawnTimer >= GameVariables.AsteroidSpawnInterval)
            {
                m_SpawnTimer = 0;
                Spawn();
            }

            // Indexed loop, not foreach -- Weapon.ApplyAsteroidDamage appends
            // fragments straight into this same list during weapon
            // resolution, and an indexed loop tolerates that the way a
            // foreach's enumerator wouldn't.
            for (int i = 0; i < m_Asteroids.Count; i++)
            {
                Asteroid asteroid = m_Asteroids[i];
                asteroid.Update(gameTime);

                if (!asteroid.IsAlive ||
                    Physics.IsOutOfBounds(asteroid.Position, m_ViewportRect, GameVariables.AsteroidDespawnMargin))
                {
                    m_Asteroids.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Spawn()
        {
            Vector2 position = GetEdgeSpawnPosition();

            // Aim roughly at a random point inside the viewport rather than
            // a fully random heading -- guarantees the asteroid actually
            // crosses visible play space instead of clipping past a corner.
            Vector2 target = new Vector2(
                Extensions.Random.Next(m_ViewportRect.Left, m_ViewportRect.Right),
                Extensions.Random.Next(m_ViewportRect.Top, m_ViewportRect.Bottom));

            AsteroidSize size = PickSize();

            m_Asteroids.Add(new Asteroid(m_GraphicsDevice, position, target - position, size));
        }

        private Vector2 GetEdgeSpawnPosition()
        {
            int margin = GameVariables.AsteroidDespawnMargin;

            switch (Extensions.Random.Next(4))
            {
                case 0: // Top
                    return new Vector2(Extensions.Random.Next(m_ViewportRect.Left, m_ViewportRect.Right), m_ViewportRect.Top - margin);

                case 1: // Bottom
                    return new Vector2(Extensions.Random.Next(m_ViewportRect.Left, m_ViewportRect.Right), m_ViewportRect.Bottom + margin);

                case 2: // Left
                    return new Vector2(m_ViewportRect.Left - margin, Extensions.Random.Next(m_ViewportRect.Top, m_ViewportRect.Bottom));

                default: // Right
                    return new Vector2(m_ViewportRect.Right + margin, Extensions.Random.Next(m_ViewportRect.Top, m_ViewportRect.Bottom));
            }
        }

        private AsteroidSize PickSize()
        {
            float total = GameVariables.AsteroidSpawnWeightLarge
                + GameVariables.AsteroidSpawnWeightMedium
                + GameVariables.AsteroidSpawnWeightSmall;

            float roll = Extensions.Random.NextFloat(0, total);

            if (roll < GameVariables.AsteroidSpawnWeightLarge)
            {
                return AsteroidSize.Large;
            }

            if (roll < GameVariables.AsteroidSpawnWeightLarge + GameVariables.AsteroidSpawnWeightMedium)
            {
                return AsteroidSize.Medium;
            }

            return AsteroidSize.Small;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Asteroid asteroid in m_Asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
