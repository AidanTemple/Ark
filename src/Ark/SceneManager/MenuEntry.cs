#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class MenuEntry
    {
        #region Private Members

        private string m_Text;

        private float m_Fade;

        private Vector2 m_Position;

        #endregion

        #region Properties

        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        #endregion

        #region Initialisation

        public MenuEntry(string text)
        {
            this.m_Text = text;
        }

        #endregion

        #region Event Handlers

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectedEntry(PlayerIndex playerIndex)
        {
            if(Selected != null)
            {
                Selected(this, new PlayerIndexEventArgs(playerIndex));
            }
        }

        #endregion

        #region Update

        public virtual void Update(Menu scene, bool selected, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if(selected)
            {
                m_Fade = Math.Min(m_Fade + deltaTime, 1);
            }
            else
            {
                m_Fade = Math.Max(m_Fade - deltaTime, 0);
            }
        }

        #endregion

        #region Draw

        public virtual void Draw(Menu scene, bool selected, GameTime gameTime)
        {
            Color color;

            if (selected)
            {
                color = Color.Yellow;
            }
            else
            {
                color = Color.White;
            }

            // Scaled by m_Fade (not an instant jump) so growing into focus
            // still transitions smoothly -- just without the old pulse's
            // continuous sine-wave oscillation. 1.1 matches the previous
            // pulse's peak size.
            float scale = 1 + 0.1f * m_Fade;

            color *= scene.TransitionAlpha;

            SceneManager manager = scene.SceneManager;
            SpriteBatch spriteBatch = manager.SpriteBatch;
            SpriteFont font = manager.MenuFont;

            if (font == null)
            {
                return;
            }

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, m_Text, m_Position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        #endregion

        #region Helper Methods

        public virtual int GetHeight(Menu scene)
        {
            return (int)scene.SceneManager.MenuFont.LineSpacing;
        }

        public virtual int GetWidth(Menu scene)
        {
            return (int)scene.SceneManager.MenuFont.MeasureString(Text).X;
        }

        #endregion
    }
}
