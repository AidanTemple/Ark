#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public abstract class Weapon
    {
        #region Private Members

        private readonly Projectile[] m_Projectiles;

        private readonly float m_FireInterval;
        private float m_FireTimer;

        #endregion

        #region Properties

        protected float Damage { get; private set; }

        protected abstract Vector2 LaunchVelocity { get; }

        #endregion

        #region Initialisation

        protected Weapon(int poolSize, float fireInterval, float damage)
        {
            m_FireInterval = fireInterval;
            Damage = damage;

            m_FireTimer = fireInterval;

            m_Projectiles = new Projectile[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                m_Projectiles[i] = CreateProjectile();
            }
        }

        protected abstract Projectile CreateProjectile();

        #endregion

        #region Update

        public bool TryFire(Vector2 position)
        {
            if (m_FireTimer < m_FireInterval)
            {
                return false;
            }

            foreach (Projectile projectile in m_Projectiles)
            {
                if (!projectile.IsAlive)
                {
                    projectile.Activate(position, LaunchVelocity);
                    m_FireTimer = 0;

                    return true;
                }
            }

            return false;
        }

        public void Update(GameTime gameTime, Rectangle viewportBounds, List<Enemy> enemies)
        {
            m_FireTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Projectile projectile in m_Projectiles)
            {
                if (!projectile.IsAlive)
                {
                    continue;
                }

                projectile.Update(gameTime);

                if (!viewportBounds.Contains(new Point((int)projectile.Position.X, (int)projectile.Position.Y)))
                {
                    projectile.IsAlive = false;
                    continue;
                }

                ResolveEffects(projectile, gameTime, enemies);
            }
        }

        /// <summary>
        /// Called every frame for every currently-alive projectile this weapon owns --
        /// not just on the frame it first hits something. Implementations decide how
        /// (and whether) to interact with the enemy list and when to kill the projectile.
        /// </summary>
        protected abstract void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies);

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in m_Projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
