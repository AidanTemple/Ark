namespace Ark
{
    // A static stat modifier -- no per-frame state, so it just leaves
    // Module.Update() at its no-op default. Read by Ship.Speed/
    // Ship.TurnRateDegrees when fitted.
    public class PropulsionModule : Module
    {
        #region Properties

        public float SpeedMultiplier { get; private set; }
        public float TurnRateMultiplier { get; private set; }

        #endregion

        #region Initialisation

        public PropulsionModule()
        {
            SpeedMultiplier = GameVariables.PropulsionSpeedMultiplier;
            TurnRateMultiplier = GameVariables.PropulsionTurnRateMultiplier;
        }

        #endregion
    }
}
