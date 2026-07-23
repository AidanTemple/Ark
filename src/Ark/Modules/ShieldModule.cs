#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    public class ShieldModule : Module
    {
        #region Private Members

        private float m_TimeSinceDamage;

        #endregion

        #region Properties

        public float Capacity { get; private set; }
        public float Current { get; private set; }

        public float Percent
        {
            get { return Capacity > 0f ? Current / Capacity * 100f : 0f; }
        }

        #endregion

        #region Initialisation

        public ShieldModule()
        {
            Capacity = GameVariables.ShieldCapacity;
            Current = Capacity;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_TimeSinceDamage += deltaTime;

            if (m_TimeSinceDamage >= GameVariables.ShieldRepairDelay)
            {
                Current = MathHelper.Min(Current + GameVariables.ShieldReplenishRate * deltaTime, Capacity);
            }
        }

        // Soaks as much of the incoming damage as available shield allows
        // and resets the repair-delay timer; returns whatever's left over
        // for the next layer (Ship.Health) to take.
        public float Absorb(float damage)
        {
            m_TimeSinceDamage = 0f;

            float absorbed = MathHelper.Min(Current, damage);
            Current -= absorbed;

            return damage - absorbed;
        }

        #endregion
    }
}
