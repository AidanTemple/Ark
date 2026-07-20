#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class GravityBombWeapon : Weapon
    {
        #region Constants

        private const int m_PoolSize = 1;

        #endregion

        #region Initialisation

        public GravityBombWeapon()
            : base(m_PoolSize, GameVariables.GravityBombFireInterval, GameVariables.GravityBombDamage)
        {
        }

        protected override Projectile CreateProjectile()
        {
            return new GravityBombProjectile(ContentManager.Torpedo);
        }

        #endregion

        #region Properties

        protected override Vector2 LaunchVelocity
        {
            get { return new Vector2(0, GameVariables.GravityBombVelocityY); }
        }

        // The bomb always kills itself on a fixed timer via Detonate() below,
        // regardless of position -- it must not be cut short by the generic
        // out-of-bounds check while still climbing through its Flying phase.
        protected override bool KillWhenOutOfBounds
        {
            get { return false; }
        }

        #endregion

        #region Update

        protected override void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies, List<Asteroid> asteroids)
        {
            GravityBombProjectile bomb = (GravityBombProjectile)projectile;

            switch (bomb.State)
            {
                case GravityBombState.Pulling:
                    UpdatePull(bomb, enemies, asteroids);
                    break;

                case GravityBombState.Detonating:
                    Detonate(bomb, enemies, asteroids);
                    break;
            }
        }

        // Recomputed every frame rather than set-and-forget, so enemies that
        // drift out of range (or the bomb dying) release them automatically --
        // no explicit "release" step needed.
        private void UpdatePull(GravityBombProjectile bomb, List<Enemy> enemies, List<Asteroid> asteroids)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsAlive)
                {
                    continue;
                }

                if (Physics.IsWithinRadius(enemy.Position, bomb.Position, GameVariables.GravityBombPullRadius))
                {
                    enemy.IsBeingPulled = true;

                    enemy.Position += Physics.CalculatePullStep(enemy.Position, bomb.Position, GameVariables.GravityBombPullSpeed);
                }
                else
                {
                    enemy.IsBeingPulled = false;
                }
            }

            // foreach is safe here -- this only mutates existing asteroids in
            // place (Position/IsBeingPulled), it never adds to the list.
            foreach (Asteroid asteroid in asteroids)
            {
                if (!asteroid.IsAlive)
                {
                    continue;
                }

                if (Physics.IsWithinRadius(asteroid.Position, bomb.Position, GameVariables.GravityBombPullRadius))
                {
                    asteroid.IsBeingPulled = true;

                    asteroid.Position += Physics.CalculatePullStep(asteroid.Position, bomb.Position, GameVariables.GravityBombPullSpeed);
                }
                else
                {
                    asteroid.IsBeingPulled = false;
                }
            }
        }

        // Runs exactly once: Weapon.Update only calls ResolveEffects while the
        // projectile is alive, and this sets IsAlive = false at the end.
        private void Detonate(GravityBombProjectile bomb, List<Enemy> enemies, List<Asteroid> asteroids)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsAlive)
                {
                    continue;
                }

                enemy.IsBeingPulled = false;

                if (Physics.IsWithinRadius(enemy.Position, bomb.Position, GameVariables.GravityBombDetonateRadius))
                {
                    ApplyDamage(enemy);
                }
            }

            // Indexed loop, not foreach -- ApplyAsteroidDamage can append a
            // fragment into this same list mid-pass, which would invalidate a
            // foreach enumerator.
            for (int i = 0; i < asteroids.Count; i++)
            {
                Asteroid asteroid = asteroids[i];

                if (!asteroid.IsAlive)
                {
                    continue;
                }

                asteroid.IsBeingPulled = false;

                if (Physics.IsWithinRadius(asteroid.Position, bomb.Position, GameVariables.GravityBombDetonateRadius))
                {
                    // GravityBombProjectile.Velocity is zeroed out once it
                    // enters Pulling, so it can't supply an impact direction
                    // the way Laser/Railgun's projectile Velocity does --
                    // use the radial direction away from the blast center.
                    Vector2 impactDirection = asteroid.Position - bomb.Position;

                    Asteroid fragment = ApplyAsteroidDamage(asteroid, impactDirection);

                    if (fragment != null)
                    {
                        asteroids.Add(fragment);
                    }
                }
            }

            Vector2 bombPosition = new Vector2(bomb.Position.X - bomb.Origin.X, bomb.Position.Y - bomb.Origin.Y);

            ParticleEffects.SpawnBurst(GameScene.Particle, bomb.Width, bomb.Height, bombPosition, 200,
                Color.Purple, Color.White, 150, ParticleType.Enemy);

            bomb.IsAlive = false;
        }

        #endregion
    }
}
