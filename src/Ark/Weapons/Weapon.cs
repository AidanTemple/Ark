#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        protected abstract Vector2 LaunchVelocity { get; }

        // Most projectiles are pure straight-line shots, so leaving the
        // viewport always means "gone for good." GravityBombWeapon overrides
        // this -- its projectile is driven by a state timer, not position,
        // and always kills itself once Detonating, so it must survive going
        // briefly out of bounds during its Flying phase instead of being cut
        // short before it ever gets there.
        protected virtual bool KillWhenOutOfBounds
        {
            get { return true; }
        }

        #endregion

        #region Initialisation

        protected Weapon(int poolSize, float fireInterval)
        {
            m_FireInterval = fireInterval;

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

        public void Update(GameTime gameTime, Rectangle viewportBounds)
        {
            m_FireTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Projectile projectile in m_Projectiles)
            {
                if (!projectile.IsAlive)
                {
                    continue;
                }

                projectile.Update(gameTime);

                if (KillWhenOutOfBounds && Physics.IsOutOfBounds(projectile.Position, viewportBounds))
                {
                    projectile.IsAlive = false;
                    continue;
                }

                OnProjectileUpdated(projectile);
            }
        }

        // Hook for weapons whose projectiles need extra per-frame handling
        // beyond moving and dying out of bounds -- e.g. GravityBombWeapon
        // killing its bomb once its detonation timer elapses. No-op by
        // default (Laser/Railgun need nothing beyond the above).
        protected virtual void OnProjectileUpdated(Projectile projectile)
        {
        }

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
