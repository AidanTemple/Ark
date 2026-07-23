namespace Ark
{
    static class GameVariables
    {
        public static float PlayerSpeed = 60.0f;

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

        // Player movement
        // Gamepad-stick cursor-nudge speed (px/sec -- deltaTime-scaled,
        // like the rest of Player's movement code).
        public static float CursorSpeed = 400f;

        // Arrive behavior: steering/turning stops inside PlayerArrivalRadius;
        // deceleration ramps in starting at PlayerSlowRadius.
        public static float PlayerArrivalRadius = 6f;
        public static float PlayerSlowRadius = 100f;

        public static float PlayerTurnRateDegrees = 90f;

        // Rotation and thrust are mutually exclusive (see
        // Player.UpdateSteering) -- the ship only turns once its velocity
        // has bled below this speed, and only thrusts once its heading is
        // within this many degrees of the destination.
        public static float PlayerBrakingSpeedThreshold = 4f;
        public static float PlayerHeadingToleranceDegrees = 2f;

        // Shared accel/decel rate: how briskly the ship can brake toward a
        // stop, or build up to (and bleed back down from) PlayerSpeed once
        // pointed the right way.
        public static float PlayerAcceleration = 90f;

        // Seconds for engine power to ramp from 0 to full, and back down
        // again once thrust is no longer commanded -- the "spool up" delay
        // before the ship actually starts responding to a new destination.
        public static float PlayerEngineSpoolUpTime = 0.8f;
    }
}
