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

                if (!railgun.BoundingRect.Intersects(enemy.BoundingRect))
                {
                    continue;
                }

                railgun.HitEnemies.Add(enemy);

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
        }

        #endregion
    }
}
