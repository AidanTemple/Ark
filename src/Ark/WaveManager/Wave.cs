#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

namespace Ark
{
    class Wave
    {
        #region Private Members

        private GraphicsDevice m_GraphicsDevice;
        private int m_EnemyCount;
        private int m_WaveNumber;
        private int m_SpawnCount = 0;

        private float m_SpawnTimer = 0;

        private bool m_HasReachedEnd;
        private bool m_IsSpawning;

        private Texture2D m_Texture;

        private List<Enemy> m_Enemies = new List<Enemy>();

        private Random m_Random =  new Random();

        #endregion

        #region Properties

        public bool IsWaveComplete
        {
            get
            {
                return (m_Enemies.Count == 0 && m_SpawnCount == m_EnemyCount);
            }
        }

        public int WaveNumber
        {
            get { return m_WaveNumber; }
        }

        public bool HasReachedEnd
        {
            get { return m_HasReachedEnd; }
            set { m_HasReachedEnd = value; }
        }

        public List<Enemy>Enemies
        {
            get { return m_Enemies; }
        }

        #endregion

        #region Initialisation

        public Wave(GraphicsDevice graphicsDevice, int waveNumber, int enemyCount, Texture2D texture)
        {
            m_GraphicsDevice = graphicsDevice;
            m_WaveNumber = waveNumber;
            m_EnemyCount = enemyCount;
            m_Texture = texture;
        }

        private void AddEnemy()
        {
            int x = m_Random.Next(30, 450);
            int y = m_Random.Next(-30, -10);

            Enemy enemy = new Enemy(m_GraphicsDevice, new Vector2(x, y), 1, GameVariables.EnemySpeed);
            m_Enemies.Add(enemy);
            m_SpawnTimer = 0;
            m_SpawnCount += 1;
        }

        public void Start()
        {
            m_IsSpawning = true;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if(m_SpawnCount == m_EnemyCount)
            {
                // Stop spawning enemies.
                m_IsSpawning = false;
            }

            if(m_IsSpawning)
            {
                m_SpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if(m_SpawnTimer > 2)
                {
                    // Add a new enemy.
                    AddEnemy();
                }
            }

            for(int i = 0; i < m_Enemies.Count; i++)
            {
                Enemy enemy = m_Enemies[i];
                enemy.Update(gameTime);

                if(enemy.Position.Y > m_GraphicsDevice.Viewport.Height)
                {
                    enemy.Position.X = m_Random.Next(30, 450);
                    enemy.Position.Y = m_Random.Next(-30, -10);
                }

                if(enemy.IsDead)
                {
                    if(enemy.CurrentHealth > 0)
                    {
                        m_HasReachedEnd = true;
                    }

                    m_Enemies.Remove(enemy);
                    i--;
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Enemy enemy in m_Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        #endregion
    }
}