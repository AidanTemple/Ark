namespace Ark
{
    // Mirrors WeaponSlot: a mount point on the ship that a Shield plugs
    // into. Shields are passive (no trigger button), but the slot keeps the
    // ship's loadout shape consistent with weapons -- swapping tiers, or a
    // ship with more than one shield slot, is a construction-time change
    // only.
    public class ShieldSlot
    {
        #region Properties

        public Shield Shield { get; private set; }

        #endregion

        #region Initialisation

        public ShieldSlot(Shield shield)
        {
            Shield = shield;
        }

        #endregion
    }
}
