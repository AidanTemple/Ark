#region Using Statements
using Microsoft.Xna.Framework;
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
            : base(m_PoolSize, GameVariables.LaserWeaponFireInterval)
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
    }
}
