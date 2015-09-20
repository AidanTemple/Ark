#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class LoadScene : Scene
    {
        #region Private Members

        private bool m_IsLoadingSlow;
        private bool m_OtherScreenHasFocus;

        private Scene[] m_Scenes;

        #endregion

        #region Initialisation

        private LoadScene(SceneManager manager, bool isLoadingSlow, Scene[] scenes)
        {
            m_IsLoadingSlow = isLoadingSlow;
            m_Scenes = scenes;

            IsSerializable = false;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
        }

        public static void Load(SceneManager manager, bool isLoadingSlow, 
            PlayerIndex? player, params Scene[] scenes)
        {
            foreach(Scene scene in manager.GetScenes())
            {
                scene.ExitScene();
            }

            LoadScene loadScene = new LoadScene(manager, isLoadingSlow, scenes);
            manager.AddScene(loadScene, player);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            if(m_OtherScreenHasFocus)
            {
                SceneManager.RemoveScene(this);

                foreach(Scene scene in m_Scenes)
                {
                    if(scene != null)
                    {
                        SceneManager.AddScene(scene, ControllingPlayer);
                    }
                }

                SceneManager.Game.ResetElapsedTime();
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if((SceneState == SceneState.Active) &&
                (SceneManager.GetScenes().Length == 1))
            {
                m_OtherScreenHasFocus = true;
            }

            if(m_IsLoadingSlow)
            {
                SpriteFont spriteFont = SceneManager.MenuFont;

                const string text = "Loading...";

                Viewport viewport = SceneManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = spriteFont.MeasureString(text);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, text, textPosition, color);
                spriteBatch.End();
            }
        }

        #endregion
    }
}
