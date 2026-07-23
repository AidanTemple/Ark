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

        private readonly float m_CapacitorCost;
        private readonly float m_ChargeTime;
        private float m_ChargeTimer;
        private bool m_IsCharging;
        private Vector2 m_ChargePosition;

        #endregion

        #region Properties

        // Short display name for HUD/UI purposes.
        public abstract string Name { get; }

        public bool IsCharging
        {
            get { return m_IsCharging; }
        }

        // Off cooldown, not already charging -- i.e. a TryFire call right
        // now would only be turned away by insufficient capacitor, not by
        // timing.
        public bool IsReady
        {
            get { return !m_IsCharging && m_FireTimer >= m_FireInterval; }
        }

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

        protected Weapon(int poolSize, float fireInterval, float capacitorCost, float chargeTime)
        {
            m_FireInterval = fireInterval;

            m_FireTimer = fireInterval;

            m_CapacitorCost = capacitorCost;
            m_ChargeTime = chargeTime;

            m_Projectiles = new Projectile[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                m_Projectiles[i] = CreateProjectile();
            }
        }

        protected abstract Projectile CreateProjectile();

        #endregion

        #region Update

        // Starts a charge if the weapon is off cooldown, not already
        // charging, a pool slot is free, and the capacitor can pay this
        // weapon's cost -- capacitor is drawn immediately (committing the
        // power up front, not on completion), so a shot that's already
        // charging can't be starved by a later shot draining the capacitor
        // out from under it. The projectile actually launches once
        // ChargeTime elapses (see Update).
        public bool TryFire(Vector2 position, CapacitorModule capacitor)
        {
            if (m_IsCharging || m_FireTimer < m_FireInterval || !HasFreeProjectile())
            {
                return false;
            }

            if (capacitor == null || !capacitor.TryConsume(m_CapacitorCost))
            {
                return false;
            }

            m_IsCharging = true;
            m_ChargeTimer = 0f;
            m_ChargePosition = position;

            return true;
        }

        public void Update(GameTime gameTime, Rectangle viewportBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_FireTimer += deltaTime;

            if (m_IsCharging)
            {
                m_ChargeTimer += deltaTime;

                if (m_ChargeTimer >= m_ChargeTime)
                {
                    Launch();
                }
            }

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

        private bool HasFreeProjectile()
        {
            foreach (Projectile projectile in m_Projectiles)
            {
                if (!projectile.IsAlive)
                {
                    return true;
                }
            }

            return false;
        }

        // Actually fires the shot charged up by TryFire -- a free slot is
        // still guaranteed here, since nothing else can draw from this
        // weapon's own pool while it's charging (TryFire refuses re-entry
        // via m_IsCharging).
        private void Launch()
        {
            foreach (Projectile projectile in m_Projectiles)
            {
                if (!projectile.IsAlive)
                {
                    projectile.Activate(m_ChargePosition, LaunchVelocity);
                    break;
                }
            }

            m_IsCharging = false;
            m_FireTimer = 0f;
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
