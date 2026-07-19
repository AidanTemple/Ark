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

        // Most projectiles are pure straight-line shots, so leaving the
        // viewport always means "gone for good." GravityBombWeapon overrides
        // this -- its projectile is driven by a state timer, not position,
        // and always kills itself in Detonate(), so it must survive going
        // briefly out of bounds during its Flying phase instead of being cut
        // short before it ever gets to pull/detonate.
        protected virtual bool KillWhenOutOfBounds
        {
            get { return true; }
        }

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

                if (KillWhenOutOfBounds && Physics.IsOutOfBounds(projectile.Position, viewportBounds))
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

        // Shared by every weapon's hit-resolution: apply this weapon's damage,
        // award score, and spawn the death burst if that damage was lethal.
        protected void ApplyDamage(Enemy enemy)
        {
            enemy.CurrentHealth -= Damage;
            GameVariables.Score += 1;

            if (enemy.CurrentHealth <= 0)
            {
                Vector2 position = new Vector2((int)enemy.Position.X - (int)enemy.Origin.X,
                    (int)enemy.Position.Y - (int)enemy.Origin.Y);

                ParticleEffects.SpawnBurst(GameScene.Particle, enemy.Width, enemy.Height, position, 120,
                    Color.DarkSlateGray, Color.DarkRed, 100, ParticleType.Enemy);
            }
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
