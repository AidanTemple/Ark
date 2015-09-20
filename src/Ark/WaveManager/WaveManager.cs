#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Ark
{
    class WaveManager
    {
        #region Private Members

        private GraphicsDevice m_GraphicsDevice;

        private Texture2D m_Texture;
        private int m_WaveCount;
        private float m_TimeSincePrevWave;
        private bool m_IsWaveComplete;

        private Queue<Wave> m_Waves = new Queue<Wave>();

        private WaveCounter m_WaveCounter;

        #endregion

        #region Properties

        public Wave CurrentWave
        {
            get { return m_Waves.Peek(); }
        }

        public List<Enemy> Enemies
        {
            get { return CurrentWave.Enemies; }
        }

        public int WaveNumber
        {
            get { return CurrentWave.WaveNumber + 1; }
        }

        public bool IsNewWave { get; set; }

        #endregion

        #region Initialisation

        public WaveManager(GraphicsDevice graphicsDevice, Texture2D texture, int waveCount)
        {
            m_GraphicsDevice = graphicsDevice;

            m_WaveCounter = new WaveCounter();

            m_IsWaveComplete = false;

            m_Texture = texture;
            m_WaveCount = waveCount;
 
            for (int i = 0; i < m_WaveCount; i++)
            {
                int enemyCount = 6;
                int modifier = (i / 6) + 1;

                Wave wave = new Wave(graphicsDevice, i, enemyCount * modifier, m_Texture);
                m_Waves.Enqueue(wave);
            }

            BeginNextWave();
        }

        private void BeginNextWave()
        {
            if(m_Waves.Count > 0)
            {
                if (CurrentWave.WaveNumber != 0)
                {
                    m_WaveCounter.IsWaveCounterEnabled = true;
                }

                m_Waves.Peek().Start();

                // Reset wave timer.
                m_TimeSincePrevWave = 0;
                m_IsWaveComplete = false;
            }
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            CurrentWave.Update(gameTime);

            if(CurrentWave.IsWaveComplete)
            {
                m_IsWaveComplete = true;
            }

            if(m_IsWaveComplete)
            {
                m_TimeSincePrevWave += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if(m_TimeSincePrevWave > GameVariables.TimeBetweenWaves)
            {
                m_Waves.Dequeue();
                BeginNextWave();
            }

            m_WaveCounter.Update(gameTime);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentWave.Draw(spriteBatch);

            if (m_WaveCounter.IsWaveCounterEnabled)
            {
                m_WaveCounter.Draw(spriteBatch, m_GraphicsDevice.Viewport, WaveNumber);
            }
        }

        #endregion
    }
}
