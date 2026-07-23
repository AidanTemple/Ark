#region Using Statements
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
#endregion

namespace Ark
{
    // Scene-scoped, not a global "load everything at boot" bag -- each region
    // is loaded by the scene that needs it (in that scene's own
    // LoadContent()) and unloaded when that scene exits. See GameScene for
    // the owning ContentManager instance.
    static class ContentManager
    {
        #region Helper Methods

        // A straight content.Load<T> throws when an .xnb is missing and
        // takes the whole scene load down with it. Swallow that here so a
        // missing asset degrades to "doesn't draw" instead of a hard crash;
        // callers just get null back, and every draw call site is expected
        // to tolerate a null Texture/Font (see Extensions.DrawSafe and the
        // null checks guarding DrawString call sites throughout).
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

        #region Game

        public static Texture2D Player { get; private set; }
        public static Texture2D Missile { get; private set; }
        public static Texture2D Torpedo { get; private set; }
        public static Texture2D Pulse { get; private set; }

        public static void LoadGame(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Player          = TryLoad<Texture2D>(content, "Textures/Player");
            Missile         = TryLoad<Texture2D>(content, "Textures/Missile");
            Torpedo         = TryLoad<Texture2D>(content, "Textures/Torpedo");
            Pulse           = TryLoad<Texture2D>(content, "Textures/Pulse");
        }

        public static void UnloadGame()
        {
            Player = null;
            Missile = null;
            Torpedo = null;
            Pulse = null;
        }

        #endregion
    }
}
