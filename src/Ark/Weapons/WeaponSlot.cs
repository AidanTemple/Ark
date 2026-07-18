#region Using Statements
using Microsoft.Xna.Framework.Input;
#endregion

namespace Ark
{
    public class WeaponSlot
    {
        #region Properties

        public Weapon Weapon { get; private set; }

        public Buttons TriggerButton { get; private set; }

        #endregion

        #region Initialisation

        public WeaponSlot(Weapon weapon, Buttons triggerButton)
        {
            Weapon = weapon;
            TriggerButton = triggerButton;
        }

        #endregion
    }
}
