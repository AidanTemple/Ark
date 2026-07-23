namespace Ark
{
    static class GameVariables
    {
        // Player weapons
        // VelocityY values are px/sec (Projectile.Update is deltaTime-scaled)
        // -- kept at their original on-screen speed, just converted from the
        // old implicit px/frame-at-30fps units (multiplied by 30) now that
        // the game runs at a locked 60fps. See Main.cs.
        public static float LaserWeaponFireInterval = 0.15f;
        public static float LaserWeaponVelocityY = -300f;

        public static float RailgunFireInterval = 0.6f;
        public static float RailgunVelocityY = -480f;

        public static float GravityBombFireInterval = 2.0f;
        public static float GravityBombVelocityY = -180f;
        public static float GravityBombFlightDuration = 0.6f;
        public static float GravityBombPullDuration = 1.2f;

        // Ship movement -- shared defaults every Ship subclass gets unless
        // it overrides the corresponding virtual property (see Ship.cs) to
        // make its own ship type feel different.
        public static float ShipSpeed = 60.0f;

        // Arrive behavior: steering/turning stops inside ShipArrivalRadius;
        // deceleration ramps in starting at ShipSlowRadius.
        public static float ShipArrivalRadius = 6f;
        public static float ShipSlowRadius = 100f;

        public static float ShipTurnRateDegrees = 90f;

        // Rotation and thrust are mutually exclusive (see
        // Ship.UpdateSteering) -- a ship only turns once its velocity has
        // bled below this speed, and only thrusts once its heading is
        // within this many degrees of the destination.
        public static float ShipBrakingSpeedThreshold = 4f;
        public static float ShipHeadingToleranceDegrees = 2f;

        // Shared accel/decel rate: how briskly a ship can brake toward a
        // stop, or build up to (and bleed back down from) Speed once
        // pointed the right way.
        public static float ShipAcceleration = 90f;

        // Seconds for engine power to ramp from 0 to full, and back down
        // again once thrust is no longer commanded -- the "spool up" delay
        // before a ship actually starts responding to a new destination.
        public static float ShipEngineSpoolUpTime = 0.8f;

        // Ship modules -- one of each fitted per slot (see Ship.cs's
        // Create*Module factory methods and GameVariables.ShipHealth
        // below). Damage that reaches a ship goes through Armor (flat
        // reduction) then Shield (absorption) before what's left comes off
        // Health.
        public static float ShipHealth = 100f;

        public static float ShieldCapacity = 30f;
        public static float ShieldReplenishRate = 6f;

        // Seconds without taking ANY damage before the shield starts
        // regenerating.
        public static float ShieldRepairDelay = 3.0f;

        // Flat amount subtracted from incoming damage, floored at 0 (armor
        // can't turn damage into healing).
        public static float ArmorDamageReduction = 2f;

        public static float CapacitorCapacity = 50f;
        public static float CapacitorRechargeRate = 5f;

        // Multiplicative bonuses applied to Ship.Speed/TurnRateDegrees when
        // a PropulsionModule is fitted.
        public static float PropulsionSpeedMultiplier = 1.2f;
        public static float PropulsionTurnRateMultiplier = 1.15f;

        // Player input
        // Gamepad-stick cursor-nudge speed (px/sec -- deltaTime-scaled,
        // like the rest of Player's movement code).
        public static float CursorSpeed = 400f;
    }
}
