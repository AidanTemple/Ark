#region Using Statements
using Microsoft.Xna.Framework;
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
            : base(m_PoolSize, GameVariables.GravityBombFireInterval)
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

        // The bomb always kills itself once Detonating (see
        // OnProjectileUpdated below), regardless of position -- it must not
        // be cut short by the generic out-of-bounds check while still
        // climbing through its Flying phase.
        protected override bool KillWhenOutOfBounds
        {
            get { return false; }
        }

        #endregion

        #region Update

        // GravityBombProjectile.Update drives its own Flying/Pulling/
        // Detonating timer internally -- this just kills it the first
        // frame it observes Detonating (there's nothing left to pull or
        // damage without an enemy/asteroid list).
        protected override void OnProjectileUpdated(Projectile projectile)
        {
            GravityBombProjectile bomb = (GravityBombProjectile)projectile;

            if (bomb.State == GravityBombState.Detonating)
            {
                bomb.IsAlive = false;
            }
        }

        #endregion
    }
}
