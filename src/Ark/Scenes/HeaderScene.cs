#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class HeaderScene : Scene
    {
        #region Private Members

        private Microsoft.Xna.Framework.Content.ContentManager m_Content;

        private Texture2D m_MonoTexture;
        private Texture2D m_NanoTexture;

        private TimeSpan m_Time = TimeSpan.FromSeconds(6.0);

        private bool CanSwitchTexture;

        #endregion

        #region Initialisation

        public HeaderScene(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            //if(m_Content == null)
            //{
            //    m_Content = new Microsoft.Xna.Framework.Content.ContentManager(SceneManager.Game.Services, "Content");
            //}

            m_Content = content;

            CanSwitchTexture = false;
        }

        public override void LoadContent()
        {
            SceneManager.Game.ResetElapsedTime();

            m_MonoTexture = m_Content.Load<Texture2D>("Textures/MonoTexture");
            m_NanoTexture = m_Content.Load<Texture2D>("Textures/NanoTexture");
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
                spriteBatch.Draw(m_MonoTexture, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(m_NanoTexture, Vector2.Zero, Color.White);
            }

            spriteBatch.End();
        }

        #endregion
    }
}