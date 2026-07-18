#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class LaserWeapon : Weapon
    {
        #region Constants

        private const int m_PoolSize = 3;

        #endregion

        #region Initialisation

        public LaserWeapon()
            : base(m_PoolSize, GameVariables.LaserWeaponFireInterval, GameVariables.LaserWeaponDamage)
        {
        }

        protected override Projectile CreateProjectile()
        {
            return new Projectile(ContentManager.Missile);
        }

        #endregion

        #region Properties

        protected override Vector2 LaunchVelocity
        {
            get { return new Vector2(0, GameVariables.LaserWeaponVelocityY); }
        }

        #endregion

        #region Update

        protected override void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.IsAlive && Physics.Overlaps(projectile.BoundingRect, enemy.BoundingRect))
                {
                    projectile.IsAlive = false;

                    enemy.CurrentHealth -= Damage;
                    GameVariables.Score += 1;

                    if (enemy.CurrentHealth <= 0)
                    {
                        Vector2 position = new Vector2((int)enemy.Position.X - (int)enemy.Origin.X,
                            (int)enemy.Position.Y - (int)enemy.Origin.Y);

                        ParticleEffects.SpawnBurst(GameScene.Particle, enemy.Width, enemy.Height, position, 120,
                            Color.DarkSlateGray, Color.DarkRed, 100, ParticleType.Enemy);
                    }

                    return;
                }
            }
        }

        #endregion
    }
}
