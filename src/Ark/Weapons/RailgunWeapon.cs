#region Using Statements
using Microsoft.Xna.Framework;
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
            : base(m_PoolSize, GameVariables.RailgunFireInterval,
                  GameVariables.RailgunCapacitorCost, GameVariables.RailgunChargeTime)
        {
        }

        protected override Projectile CreateProjectile()
        {
            return new Projectile(ContentManager.Pulse);
        }

        #endregion

        #region Properties

        public override string Name
        {
            get { return "Railgun"; }
        }

        protected override Vector2 LaunchVelocity
        {
            get { return new Vector2(0, GameVariables.RailgunVelocityY); }
        }

        #endregion
    }
}
