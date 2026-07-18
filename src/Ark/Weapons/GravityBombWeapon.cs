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

        #endregion

        #region Update

        protected override void ResolveEffects(Projectile projectile, GameTime gameTime, List<Enemy> enemies)
        {
            GravityBombProjectile bomb = (GravityBombProjectile)projectile;

            switch (bomb.State)
            {
                case GravityBombState.Pulling:
                    UpdatePull(bomb, enemies);
                    break;

                case GravityBombState.Detonating:
                    Detonate(bomb, enemies);
                    break;
            }
        }

        // Recomputed every frame rather than set-and-forget, so enemies that
        // drift out of range (or the bomb dying) release them automatically --
        // no explicit "release" step needed.
        private void UpdatePull(GravityBombProjectile bomb, List<Enemy> enemies)
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
        }

        // Runs exactly once: Weapon.Update only calls ResolveEffects while the
        // projectile is alive, and this sets IsAlive = false at the end.
        private void Detonate(GravityBombProjectile bomb, List<Enemy> enemies)
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

            Vector2 bombPosition = new Vector2(bomb.Position.X - bomb.Origin.X, bomb.Position.Y - bomb.Origin.Y);

            ParticleEffects.SpawnBurst(GameScene.Particle, bomb.Width, bomb.Height, bombPosition, 200,
                Color.Purple, Color.White, 150, ParticleType.Enemy);

            bomb.IsAlive = false;
        }

        #endregion
    }
}
