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

        protected override void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies, List<Asteroid> asteroids)
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

            // foreach is safe here (unlike Railgun/GravityBomb) because this
            // method always returns right after its one hit -- the
            // enumerator's MoveNext() never runs again, so appending a
            // fragment to this same list below can't invalidate it.
            foreach (Asteroid asteroid in asteroids)
            {
                if (asteroid.IsAlive && Physics.Overlaps(projectile.BoundingRect, asteroid.BoundingRect))
                {
                    projectile.IsAlive = false;

                    Asteroid fragment = ApplyAsteroidDamage(asteroid, projectile.Velocity);

                    if (fragment != null)
                    {
                        asteroids.Add(fragment);
                    }

                    return;
                }
            }
        }

        #endregion
    }
}
