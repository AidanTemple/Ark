#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    public class ArmorModule : Module
    {
        #region Properties

        public float DamageReduction { get; private set; }

        #endregion

        #region Initialisation

        public ArmorModule()
        {
            DamageReduction = GameVariables.ArmorDamageReduction;
        }

        #endregion

        #region Helper Methods

        // Flat reduction, floored at 0 so armor can't turn damage into
        // healing.
        public float Reduce(float damage)
        {
            return MathHelper.Max(damage - DamageReduction, 0f);
        }

        #endregion
    }
}
