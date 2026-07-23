#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public enum GravityBombState
    {
        Flying,
        Pulling,
        Detonating
    }

    public class GravityBombProjectile : Projectile
    {
        #region Private Members

        private float m_StateTimer;

        #endregion

        #region Properties

        public GravityBombState State { get; private set; }

        #endregion

        #region Initialisation

        public GravityBombProjectile(Texture2D texture)
            : base(texture)
        {
        }

        public override void Activate(Vector2 position, Vector2 velocity)
        {
            base.Activate(position, velocity);

            State = GravityBombState.Flying;
            m_StateTimer = 0;
        }

        #endregion

        #region Update

        // State *timing* only lives here -- GravityBombWeapon.OnProjectileUpdated
        // reads State to know when to kill this projectile once Detonating.
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            m_StateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (State)
            {
                case GravityBombState.Flying:
                    if (m_StateTimer >= GameVariables.GravityBombFlightDuration)
                    {
                        State = GravityBombState.Pulling;
                        m_StateTimer = 0;

                        // Freeze in place -- base.Update() only moves us via Velocity.
                        Velocity = Vector2.Zero;
                    }
                    break;

                case GravityBombState.Pulling:
                    if (m_StateTimer >= GameVariables.GravityBombPullDuration)
                    {
                        State = GravityBombState.Detonating;
                        m_StateTimer = 0;
                    }
                    break;

                case GravityBombState.Detonating:
                    // GravityBombWeapon.OnProjectileUpdated kills this
                    // projectile the first time it observes this state.
                    break;
            }
        }

        #endregion
    }
}
