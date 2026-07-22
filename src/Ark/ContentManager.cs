#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    // Scene-scoped, not a global "load everything at boot" bag -- each region
    // is loaded by the scene that needs it (in that scene's own
    // LoadContent()) and unloaded when that scene exits. See Menu and
    // GameScene for the owning ContentManager instances.
    static class ContentManager
    {
        #region Menu

        // Menu no longer loads/draws a background texture -- see Menu.cs.
        // Kept as no-ops (rather than removing the calls at the Menu.cs call
        // sites) so LoadContent()/UnloadContent() there don't need touching
        // if menu content returns later.
        public static void LoadMenu(Microsoft.Xna.Framework.Content.ContentManager content)
        {
        }

        public static void UnloadMenu()
        {
        }

        #endregion

        #region Game

        public static Texture2D Player { get; private set; }
        public static Texture2D Missile { get; private set; }
        public static Texture2D Torpedo { get; private set; }
        public static Texture2D Pulse { get; private set; }
        public static Texture2D Enemy { get; private set; }
        public static Texture2D EnemyLaser { get; private set; }
        public static Texture2D LineParticle { get; private set; }
        public static Texture2D StatusBar { get; private set; }

        // No Asteroid_Large/Medium/Small.xnb exist in Content/ yet (no source
        // art, unlike the rest of this checked-in-precompiled folder) --
        // left declared (Asteroid.cs still references them) but not loaded,
        // so AsteroidManager's spawning stays disabled until real art lands.
        public static Texture2D AsteroidLarge { get; private set; }
        public static Texture2D AsteroidMedium { get; private set; }
        public static Texture2D AsteroidSmall { get; private set; }

        public static SpriteFont Game0Font { get; private set; }
        public static SpriteFont LargeFont { get; private set; }
        public static SpriteFont MediumFont { get; private set; }

        public static void LoadGame(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Player          = content.Load<Texture2D>("Textures/Player");
            Missile         = content.Load<Texture2D>("Textures/Missile");
            Torpedo         = content.Load<Texture2D>("Textures/Torpedo");
            Pulse           = content.Load<Texture2D>("Textures/Pulse");
            Enemy           = content.Load<Texture2D>("Textures/Enemy_Normal");
            EnemyLaser      = content.Load<Texture2D>("Textures/Enemy_Laser");
            LineParticle    = content.Load<Texture2D>("Textures/LineParticle");
            StatusBar       = content.Load<Texture2D>("Textures/StatusBar");

            Game0Font       = content.Load<SpriteFont>("Fonts/game0");
            LargeFont       = content.Load<SpriteFont>("Fonts/LargeFont");
            MediumFont      = content.Load<SpriteFont>("Fonts/mediumFont");
        }

        public static void UnloadGame()
        {
            Player = null;
            Missile = null;
            Torpedo = null;
            Pulse = null;
            Enemy = null;
            EnemyLaser = null;
            LineParticle = null;
            StatusBar = null;

            Game0Font = null;
            LargeFont = null;
            MediumFont = null;
        }

        #endregion
    }
}
