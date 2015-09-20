#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    class WaveCounter
    {
        #region Private Members

        private int m_Blinks;

        private float m_BlinkTime;
        private float m_CurrentBlinkTime;

        private bool m_Blink;

        #endregion

        #region Properties

        public bool IsWaveCounterEnabled { get; set; }

        #endregion

        #region Initialisation

        public WaveCounter()
        {
            m_Blinks = 0;
            m_BlinkTime = 1f;
            m_CurrentBlinkTime = 0f;

            m_Blink = false;
            IsWaveCounterEnabled = false;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if(IsWaveCounterEnabled)
            {
                m_CurrentBlinkTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(m_CurrentBlinkTime > m_BlinkTime)
                {
                    m_Blink = !m_Blink;
                    m_CurrentBlinkTime -= m_BlinkTime;
                    m_Blinks += 1;
                }

                if(m_Blinks > 5)
                {
                    IsWaveCounterEnabled = false;
                    m_Blinks = 0;
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch, Viewport viewport, int waveNumber)
        {
            if (m_Blink)
            {
                string count = waveNumber.ToString("00");

                ContentManager.LargeFont.Spacing = 50;

                Vector2 position = Extensions.CenterString(ContentManager.LargeFont,
                    count, viewport.Width, viewport.Height);

                spriteBatch.DrawString(ContentManager.MediumFont, "WAVE", new Vector2(100, 280), Color.White);
                spriteBatch.DrawString(ContentManager.LargeFont, count, position + new Vector2(0, 30), Color.White);
            }
        }

        #endregion
    }
}