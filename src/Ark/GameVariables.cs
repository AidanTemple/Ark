namespace Ark
{
    static class GameVariables
    {
        public static int Score = 0;

        public static float TimeBetweenWaves = 5.0f;
        public static float EnemySpeed = 1.5f;
        public static float PlayerSpeed = 250.0f;
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
    }
}