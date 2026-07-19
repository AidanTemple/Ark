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
    }
}