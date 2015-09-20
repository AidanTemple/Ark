#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    static class ContentManager
    {
        #region Textures

        public static Texture2D MonoTexture { get; private set; }
        public static Texture2D NanoTexture { get; private set; }
        public static Texture2D MenuBackground { get; private set; }
        public static Texture2D ControlTexture { get; private set; }
        public static Texture2D StatTexture { get; private set; }
        public static Texture2D BlankTexture { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Background_001 { get; private set; }
        public static Texture2D Background_002 { get; private set; }
        public static Texture2D Background_003 { get; private set; }
        public static Texture2D Background_004 { get; private set; }
        public static Texture2D Missile { get; private set; }
        public static Texture2D Torpedo { get; private set; }
        public static Texture2D Pulse { get; private set; }
        public static Texture2D Enemy { get; private set; }
        public static Texture2D LineParticle { get; private set; }
        public static Texture2D StatusBar { get; private set; }
        public static Texture2D EnemyLaser { get; private set; }

        #endregion

        #region Fonts

        public static SpriteFont MenuFont { get; private set; }
        public static SpriteFont SmallFont { get; private set; }
        public static SpriteFont MediumFont { get; private set; }
        public static SpriteFont LargeFont { get; private set; }
        public static SpriteFont IntroFont { get; private set; }
        public static SpriteFont Game0Font { get; private set; }

        #endregion

        public static void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            #region Textures

            MonoTexture         = Content.Load<Texture2D>("Textures/MonoTexture");
            NanoTexture         = Content.Load<Texture2D>("Textures/NanoTexture");
            MenuBackground      = Content.Load<Texture2D>("Textures/MenuBackground");
            ControlTexture      = Content.Load<Texture2D>("Textures/backgrounds/Controls");
            StatTexture         = Content.Load<Texture2D>("Textures/Backgrounds/GameOver");
            BlankTexture        = Content.Load<Texture2D>("Textures/blank");
            Player              = Content.Load<Texture2D>("Textures/Player");
            Background_001      = Content.Load<Texture2D>("Textures/Backgrounds/background1");
            Background_002      = Content.Load<Texture2D>("Textures/Backgrounds/background2");
            Background_003      = Content.Load<Texture2D>("Textures/Backgrounds/background3");
            Background_004      = Content.Load<Texture2D>("Textures/Backgrounds/background4");
            Missile             = Content.Load<Texture2D>("Textures/Missile");
            Torpedo             = Content.Load<Texture2D>("Textures/Torpedo");
            Pulse               = Content.Load<Texture2D>("Textures/Pulse");
            Enemy               = Content.Load<Texture2D>("Textures/Enemy_Normal");
            LineParticle        = Content.Load<Texture2D>("Textures/LineParticle");
            StatusBar           = Content.Load<Texture2D>("Textures/StatusBar");
            EnemyLaser          = Content.Load<Texture2D>("Textures/Enemy_Laser");

            #endregion

            #region Fonts

            MenuFont            = Content.Load<SpriteFont>("Fonts/menu");
            SmallFont           = Content.Load<SpriteFont>("Fonts/smallFont");
            MediumFont          = Content.Load<SpriteFont>("Fonts/mediumFont");
            LargeFont           = Content.Load<SpriteFont>("Fonts/largeFont");
            IntroFont           = Content.Load<SpriteFont>("Fonts/intro");
            Game0Font           = Content.Load<SpriteFont>("Fonts/game0");

            #endregion
        }
    }
}
