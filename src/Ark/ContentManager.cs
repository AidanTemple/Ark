#region Using Statements
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
#endregion

namespace Ark
{
    // Scene-scoped, not a global "load everything at boot" bag -- each region
    // is loaded by the scene that needs it (in that scene's own
    // LoadContent()) and unloaded when that scene exits. See Menu and
    // GameScene for the owning ContentManager instances.
    static class ContentManager
    {
        #region Helper Methods

        // Content is missing more often than this project would like (see
        // the Asteroid_Large/Medium/Small comment below) -- a straight
        // content.Load<T> throws and takes the whole scene load down with
        // it. Swallow that here so one missing asset degrades to "doesn't
        // draw" instead of a hard crash; callers just get null back, and
        // every draw call site is expected to tolerate a null Texture/Font
        // (see Extensions.DrawSafe and the null checks guarding DrawString
        // call sites throughout).
        public static T TryLoad<T>(Microsoft.Xna.Framework.Content.ContentManager content, string assetName) where T : class
        {
            try
            {
                return content.Load<T>(assetName);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Content load failed for '{assetName}': {e.Message}");
                return null;
            }
        }

        #endregion

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
            Player          = TryLoad<Texture2D>(content, "Textures/Player");
            Missile         = TryLoad<Texture2D>(content, "Textures/Missile");
            Torpedo         = TryLoad<Texture2D>(content, "Textures/Torpedo");
            Pulse           = TryLoad<Texture2D>(content, "Textures/Pulse");
            Enemy           = TryLoad<Texture2D>(content, "Textures/Enemy_Normal");
            EnemyLaser      = TryLoad<Texture2D>(content, "Textures/Enemy_Laser");
            LineParticle    = TryLoad<Texture2D>(content, "Textures/LineParticle");
            StatusBar       = TryLoad<Texture2D>(content, "Textures/StatusBar");

            Game0Font       = TryLoad<SpriteFont>(content, "Fonts/game0");
            LargeFont       = TryLoad<SpriteFont>(content, "Fonts/LargeFont");
            MediumFont      = TryLoad<SpriteFont>(content, "Fonts/mediumFont");
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
