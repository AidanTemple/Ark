#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class RailgunWeapon : Weapon
    {
        #region Constants

        private const int m_PoolSize = 2;

        #endregion

        #region Initialisation

        public RailgunWeapon()
            : base(m_PoolSize, GameVariables.RailgunFireInterval, GameVariables.RailgunDamage)
        {
        }

        protected override Projectile CreateProjectile()
        {
            return new RailgunProjectile(ContentManager.Pulse);
        }

        #endregion

        #region Properties

        protected override Vector2 LaunchVelocity
        {
            get { return new Vector2(0, GameVariables.RailgunVelocityY); }
        }

        #endregion

        #region Update

        // Railgun shots pierce -- they never die on hit here, only when they
        // leave the viewport (handled generically by Weapon.Update). Each
        // projectile tracks which enemies it has already damaged this flight
        // so a piercing shot doesn't re-hit the same enemy every frame it
        // overlaps it.
        protected override void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies)
        {
            RailgunProjectile railgun = (RailgunProjectile)projectile;

            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsAlive || railgun.HitEnemies.Contains(enemy))
                {
                    continue;
                }

                if (!Physics.Overlaps(railgun.BoundingRect, enemy.BoundingRect))
                {
                    continue;
                }

                railgun.HitEnemies.Add(enemy);

                ApplyDamage(enemy);
            }
        }

        #endregion
    }
}
