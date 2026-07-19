#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    // Scene-scoped, not a global "load everything at boot" bag -- each region
    // is loaded by the scene that needs it (in that scene's own
    // LoadContent()) and unloaded when that scene exits. See HeaderScene,
    // Menu, and GameScene for the owning ContentManager instances.
    static class ContentManager
    {
        #region Header

        public static Texture2D MonoTexture { get; private set; }
        public static Texture2D NanoTexture { get; private set; }

        public static void LoadHeader(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            MonoTexture = content.Load<Texture2D>("Textures/MonoTexture");
            NanoTexture = content.Load<Texture2D>("Textures/NanoTexture");
        }

        public static void UnloadHeader()
        {
            MonoTexture = null;
            NanoTexture = null;
        }

        #endregion

        #region Menu

        public static Texture2D MenuBackground { get; private set; }

        public static void LoadMenu(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            MenuBackground = content.Load<Texture2D>("Textures/Backgrounds/background4");
        }

        public static void UnloadMenu()
        {
            MenuBackground = null;
        }

        #endregion

        #region Game

        public static Texture2D Player { get; private set; }
        public static Texture2D GameBackground { get; private set; }
        public static Texture2D Missile { get; private set; }
        public static Texture2D Torpedo { get; private set; }
        public static Texture2D Pulse { get; private set; }
        public static Texture2D Enemy { get; private set; }
        public static Texture2D EnemyLaser { get; private set; }
        public static Texture2D LineParticle { get; private set; }
        public static Texture2D StatusBar { get; private set; }

        public static SpriteFont Game0Font { get; private set; }
        public static SpriteFont LargeFont { get; private set; }
        public static SpriteFont MediumFont { get; private set; }

        public static void LoadGame(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Player          = content.Load<Texture2D>("Textures/Player");
            GameBackground  = content.Load<Texture2D>("Textures/Backgrounds/background1");
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
            GameBackground = null;
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
