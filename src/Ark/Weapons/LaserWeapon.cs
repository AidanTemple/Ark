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

                    ApplyDamage(enemy);

                    return;
                }
            }
        }

        #endregion
    }
}
