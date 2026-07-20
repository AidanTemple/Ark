#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    public enum ShieldTier
    {
        Basic,
        Advanced,
        Elite
    }

    public class Shield
    {
        #region Private Members

        private float m_TimeSinceDamage;

        #endregion

        #region Properties

        public ShieldTier Tier { get; private set; }

        public float Capacity { get; private set; }

        public float Current { get; private set; }

        // Charge restored per second once the repair delay has elapsed --
        // this is the stat that differs per tier.
        public float ReplenishRate { get; private set; }

        public float Percent
        {
            get { return Capacity > 0 ? (Current / Capacity) * 100f : 0f; }
        }

        #endregion

        #region Initialisation

        public Shield(ShieldTier tier)
        {
            Tier = tier;

            switch (tier)
            {
                case ShieldTier.Elite:
                    Capacity = GameVariables.ShieldCapacityElite;
                    ReplenishRate = GameVariables.ShieldReplenishRateElite;
                    break;

                case ShieldTier.Advanced:
                    Capacity = GameVariables.ShieldCapacityAdvanced;
                    ReplenishRate = GameVariables.ShieldReplenishRateAdvanced;
                    break;

                default:
                    Capacity = GameVariables.ShieldCapacityBasic;
                    ReplenishRate = GameVariables.ShieldReplenishRateBasic;
                    break;
            }

            Current = Capacity;

            // Start past the delay so a fresh shield isn't sitting out an
            // unearned 3-second penalty before its first repair tick.
            m_TimeSinceDamage = GameVariables.ShieldRepairDelay;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            m_TimeSinceDamage += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_TimeSinceDamage >= GameVariables.ShieldRepairDelay && Current < Capacity)
            {
                Current = MathHelper.Min(Capacity,
                    Current + ReplenishRate * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        #endregion

        #region Helper Methods

        // Soaks as much of the hit as the remaining charge allows and
        // returns the excess, which the caller applies to hull health --
        // health is never touched while any charge remains, but a hit
        // bigger than what's left does carry its remainder through.
        //
        // Always resets the repair timer, even at zero charge: the 3-second
        // "no damage taken" window is about the ship being under fire, not
        // about whether this shield happened to soak anything.
        public float Absorb(float damage)
        {
            m_TimeSinceDamage = 0;

            float absorbed = MathHelper.Min(Current, damage);

            Current -= absorbed;

            return damage - absorbed;
        }

        #endregion
    }
}
