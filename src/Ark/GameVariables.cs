namespace Ark
{
    static class GameVariables
    {
        public static int Score = 0;

        public static float TimeBetweenWaves = 5.0f;

        // Slowed for weightier, capital-ship-style combat -- see the
        // "Enemy AI" and "Player movement" sections below for the rest of
        // that retuning.
        public static float EnemySpeed = 0.7f;
        public static float PlayerSpeed = 120.0f;

        public static float BackgroundScrollSpeed = 30.0f;
        public static float LaserDamage = 0.5f;

        public static int EnemySpawnMinX = 30;
        public static int EnemySpawnMaxX = 450;
        public static int EnemySpawnMinY = -30;
        public static int EnemySpawnMaxY = -10;

        // Player weapons
        public static float LaserWeaponFireInterval = 0.15f;
        public static float LaserWeaponVelocityY = -10f;
        public static float LaserWeaponDamage = 1f;

        public static float RailgunFireInterval = 0.6f;
        public static float RailgunVelocityY = -16f;
        public static float RailgunDamage = 3f;

        public static float GravityBombFireInterval = 2.0f;
        public static float GravityBombVelocityY = -6f;
        public static float GravityBombFlightDuration = 0.6f;
        public static float GravityBombPullRadius = 160f;
        public static float GravityBombPullSpeed = 4f;
        public static float GravityBombPullDuration = 1.2f;
        public static float GravityBombDetonateRadius = 100f;
        public static float GravityBombDamage = 10f;

        // Asteroids
        public static float AsteroidHealthLarge = 30f;
        public static float AsteroidHealthMedium = 15f;
        public static float AsteroidHealthSmall = 6f;

        public static float AsteroidSpeedMinLarge = 0.5f;
        public static float AsteroidSpeedMaxLarge = 1.0f;
        public static float AsteroidSpeedMinMedium = 1.0f;
        public static float AsteroidSpeedMaxMedium = 1.8f;
        public static float AsteroidSpeedMinSmall = 1.8f;
        public static float AsteroidSpeedMaxSmall = 2.8f;

        public static float AsteroidCollisionDamageLarge = 40f;
        public static float AsteroidCollisionDamageMedium = 25f;
        public static float AsteroidCollisionDamageSmall = 12f;

        public static float AsteroidSpawnInterval = 3.5f;
        public static float AsteroidSpawnWeightLarge = 15f;
        public static float AsteroidSpawnWeightMedium = 35f;
        public static float AsteroidSpawnWeightSmall = 50f;

        // How far a fragment's launch direction can randomly deviate from
        // the impact direction, on either side.
        public static float AsteroidFragmentAngleSpreadDegrees = 50f;

        // How far outside the viewport an asteroid can sit before it's
        // despawned -- must be generous enough that one spawned just off an
        // edge isn't immediately killed by the same check.
        public static int AsteroidDespawnMargin = 80;

        // Shields
        // Seconds without taking ANY damage (shield or health) before the
        // shield starts repairing.
        public static float ShieldRepairDelay = 3.0f;

        public static float ShieldCapacityBasic = 30f;
        public static float ShieldReplenishRateBasic = 6f;

        public static float ShieldCapacityAdvanced = 60f;
        public static float ShieldReplenishRateAdvanced = 10f;

        public static float ShieldCapacityElite = 100f;
        public static float ShieldReplenishRateElite = 18f;

        // Enemy AI
        // Fire timing -- promoted from a private const in Enemy.cs and
        // lengthened (was 0.25s flat) for slower, more deliberate exchanges,
        // with jitter added so it's not perfectly metronomic.
        public static float EnemyLaserFireInterval = 1.0f;
        public static float EnemyFireIntervalJitter = 0.3f;

        // Replaces the old hardcoded enemy.Origin.X firing-arc tolerance in
        // GameScene's fire-gate check with a wider, tunable column.
        public static float EnemyFireColumnHalfWidth = 40f;

        // Evasion: a Laser/Railgun shot only counts as a threat worth
        // dodging if its predicted closest-approach point comes within this
        // distance, and that closest approach is no more than this many
        // frames away (a shot that's technically on a collision course but
        // won't arrive for a long time isn't reacted to yet).
        public static float EnemyEvasionRadius = 120f;
        public static float EnemyEvasionLookaheadFrames = 60f;
        public static float EnemyEvasionSpeed = 1.2f;

        // How quickly lateral velocity eases toward its current target
        // (evasion or ambient wander) each frame -- a flat per-frame blend
        // rather than a seconds-based smoothing constant, since the fixed
        // 30fps timestep makes those equivalent and this stays consistent
        // with the rest of the codebase's per-frame-delta convention.
        // ~0.033 approximates a ~1 second time constant at 30fps.
        public static float EnemySteeringBlendPerFrame = 0.033f;

        // Ambient side-to-side drift when no threat is present.
        public static float EnemyWanderSpeed = 0.4f;
        public static float EnemyWanderFrequency = 0.02f;

        // Player movement
        // Gamepad-stick cursor-nudge speed (px/sec -- Player's movement
        // code is already deltaTime-scaled, unlike most of the codebase).
        public static float CursorSpeed = 400f;

        // Arrive behavior: steering/turning stops inside PlayerArrivalRadius;
        // deceleration ramps in starting at PlayerSlowRadius.
        public static float PlayerArrivalRadius = 6f;
        public static float PlayerSlowRadius = 100f;

        public static float PlayerTurnRateDegrees = 90f;
        public static float PlayerAcceleration = 200f;

        // Flat per-frame velocity decay while coasting to a stop at the
        // destination.
        public static float PlayerDragFactor = 0.9f;
    }
}