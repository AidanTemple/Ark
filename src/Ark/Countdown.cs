#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    public class Countdown
    {
        #region Private Members

        private string m_Text;
        private string[] m_CountdownText;

        private int m_Blinks;

        private float m_CountBlinkTime;
        private float m_CurrentCountBlinkTime;

        private bool m_CountBlink;
        private bool m_EnableCountDown;

        #endregion

        #region Properties

        public bool IsCountingDown { get; set; }

        #endregion

        #region Initialisation

        public Countdown()
        {
            m_Text = "";

            m_CountdownText = new string[]
            {
                "3", "", "2", "", "1", "", "Go!", ""
            };

            m_Blinks = 0;
            m_CountBlinkTime = 1f;

            m_CountBlink = false;
            m_EnableCountDown = true;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (m_EnableCountDown)
            {
                IsCountingDown = true;

                m_CurrentCountBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (m_CurrentCountBlinkTime > m_CountBlinkTime)
                {
                    m_CountBlink = !m_CountBlink;
                    m_CurrentCountBlinkTime -= m_CountBlinkTime;

                    m_Text = m_CountdownText[m_Blinks];

                    m_Blinks += 1;
                }

                if (m_Blinks > 7)
                {
                    m_EnableCountDown = false;
                    IsCountingDown = false;
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (IsCountingDown)
            {
                Vector2 position = Extensions.CenterString(ContentManager.LargeFont,
                    m_Text, viewport.Width, viewport.Height);

                spriteBatch.DrawString(ContentManager.LargeFont, m_Text, position, Color.White);
            }
        }

        #endregion
    }
}