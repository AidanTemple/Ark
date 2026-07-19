#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class HeaderScene : Scene
    {
        #region Private Members

        private Microsoft.Xna.Framework.Content.ContentManager m_Content;

        private TimeSpan m_Time = TimeSpan.FromSeconds(6.0);

        private bool CanSwitchTexture;

        #endregion

        #region Initialisation

        public HeaderScene()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            CanSwitchTexture = false;
        }

        public override void LoadContent()
        {
            SceneManager.Game.ResetElapsedTime();

            m_Content = new Microsoft.Xna.Framework.Content.ContentManager(SceneManager.Game.Services, "Content");

            ContentManager.LoadHeader(m_Content);
        }

        public override void UnloadContent()
        {
            m_Content.Unload();

            ContentManager.UnloadHeader();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            m_Time -= gameTime.ElapsedGameTime;

            if(m_Time <= TimeSpan.FromSeconds(3.0))
            {
                CanSwitchTexture = true;
            }

            if (m_Time < TimeSpan.Zero)
            {
                SceneManager.RemoveScene(this);
                SceneManager.AddScene(new MenuScene(), null);
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            if (TransitionPosition > 0)
            {
                SceneManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
            }

            spriteBatch.Begin();

            if(!CanSwitchTexture)
            {
                spriteBatch.Draw(ContentManager.MonoTexture, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(ContentManager.NanoTexture, Vector2.Zero, Color.White);
            }

            spriteBatch.End();
        }

        #endregion
    }
}