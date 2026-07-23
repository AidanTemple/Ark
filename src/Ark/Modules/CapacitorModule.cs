#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    public class CapacitorModule : Module
    {
        #region Properties

        public float Capacity { get; private set; }
        public float Current { get; private set; }

        public float Percent
        {
            get { return Capacity > 0f ? Current / Capacity * 100f : 0f; }
        }

        #endregion

        #region Initialisation

        public CapacitorModule()
        {
            Capacity = GameVariables.CapacitorCapacity;
            Current = Capacity;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Current = MathHelper.Min(Current + GameVariables.CapacitorRechargeRate * deltaTime, Capacity);
        }

        #endregion

        #region Helper Methods

        // Draws from the pool if enough is available; returns whether the
        // draw succeeded. Nothing currently calls this -- reserved for a
        // future capacitor-consuming system (e.g. weapons or an active
        // module) to hook into.
        public bool TryConsume(float amount)
        {
            if (Current < amount)
            {
                return false;
            }

            Current -= amount;

            return true;
        }

        #endregion
    }
}
