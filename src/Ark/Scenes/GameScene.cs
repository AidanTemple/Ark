#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace Ark
{
    class GameScene : Scene
    {
        #region Private Members

        private Random m_Random = new Random();

        private Background m_Background;
        private Player m_Player;

        private WaveManager m_WaveManager;

        private StatusBar m_HealthBar;

        private Countdown m_Countdown;

        #endregion

        #region Properties

        public static ParticleManager<ParticleState> Particle { get; private set; }

        #endregion

        #region Initialisation

        public GameScene()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            SceneManager.Game.ResetElapsedTime();

            Reset();
        }

        private void Reset()
        {
            GameVariables.Score = 0;

            m_Background = new Background(SceneManager.GraphicsDevice);

            m_Countdown = new Countdown();

            m_Player = new Player(SceneManager.GraphicsDevice);
            m_WaveManager = new WaveManager(SceneManager.GraphicsDevice, ContentManager.Enemy, 24);

            m_HealthBar = new StatusBar();
            m_HealthBar.Percent = m_Player.Health;

            Particle = new ParticleManager<ParticleState>(1024 * 20, ParticleState.Update);
        }

        public override void UnloadContent()
        {
            
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            m_Background.Update(gameTime);
            m_Countdown.Update(gameTime);

            if (!m_Countdown.IsCountingDown)
            {
                HandleCollisions();

                m_Player.Update(gameTime);

                m_WaveManager.Update(gameTime);

                Particle.Update();

                m_HealthBar.Percent = m_Player.Health;
                m_HealthBar.Update();

                if (m_Player.Health <= 0 && m_Player.IsAlive)
                {
                    Reset();
                }

                foreach (Enemy enemy in m_WaveManager.Enemies)
                {
                    if (enemy.IsAlive)
                    {
                        if (m_Player.IsAlive && enemy.IsInRange(m_Player.Position))
                        {
                            if (m_Player.Position.X > enemy.Position.X - enemy.Origin.X
                                && m_Player.Position.X < enemy.Position.X + enemy.Origin.X)
                            {
                                enemy.FireLaser();
                            }
                        }
                    }
                }
            }
        }

        public override void UpdateInput(InputState input)
        {
            if(input != null)
            {
                PlayerIndex player;

                if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
                {
                    SceneManager.RemoveScene(this);
                    SceneManager.AddScene(new MenuScene(), ControllingPlayer);
                }
            }
        }

        private void HandleCollisions()
        {
            foreach (Missile missile in m_Player.Missiles)
            {
                foreach (Enemy enemy in m_WaveManager.Enemies)
                {
                    if (missile.IsAlive && enemy.IsAlive && missile.BoundingRect.Intersects(enemy.BoundingRect))
                    {
                        missile.IsAlive = false;
                            
                        enemy.CurrentHealth -= 1;
                        GameVariables.Score += 1;

                        if (enemy.CurrentHealth <= 0)
                        {
                            Vector2 position = new Vector2((int)enemy.Position.X - (int)enemy.Origin.X,
                                (int)enemy.Position.Y - (int)enemy.Origin.Y);

                            RemoveEntity(enemy.Width, enemy.Height, position, 120,
                                    Color.DarkSlateGray, Color.DarkRed, 100, ParticleType.Enemy);
                        }

                        break;
                    }
                }
            }

            foreach (Enemy enemy in m_WaveManager.Enemies)
            {
                foreach (Laser laser in enemy.Lasers)
                {
                    if (laser.IsAlive && laser.BoundingRect.Intersects(m_Player.BoundingRect))
                    {
                        laser.IsAlive = false;
                        m_Player.Health -= laser.Damage;

                        break;
                    }
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            spriteBatch.Begin();

            m_Background.Draw(SceneManager.SpriteBatch);
            m_Player.Draw(SceneManager.SpriteBatch);
            m_WaveManager.Draw(spriteBatch);
            Particle.Draw(SceneManager.SpriteBatch);
            m_HealthBar.Draw(SceneManager.SpriteBatch);

            Vector2 size = ContentManager.Game0Font.MeasureString(m_WaveManager.WaveNumber.ToString());

            spriteBatch.DrawString(ContentManager.Game0Font, m_WaveManager.WaveNumber.ToString(), 
                new Vector2(SceneManager.GraphicsDevice.Viewport.Width - size.X, 15), Color.White);

            m_Countdown.Draw(spriteBatch, SceneManager.GraphicsDevice.Viewport);

            DrawGUI(spriteBatch);

            spriteBatch.End();

            if (TransitionPosition > 0)
            {
                SceneManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
            }
        }

        private void DrawGUI(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(ContentManager.Game0Font, GameVariables.Score.ToString(), new Vector2(15, 15), Color.White);
        }

        #endregion

        #region Helper Methods

        private void RemoveEntity(int width, int height, Vector2 position, int particleCount,
            Color colorA, Color colorB, int duration, ParticleType type)
        {
            Vector2 pos = new Vector2(position.X + width / 2, position.Y + height / 2);

            for (int i = 0; i < particleCount; i++)
            {
                float speed = 18f * (1f - 1 / m_Random.NextFloat(1f, 10f));

                var state = new ParticleState()
                {
                    Velocity = m_Random.NextVector2(speed, speed),
                    Type = type,
                    LengthMultiplier = 1f
                };

                Color color = Color.Lerp(colorA, colorB, m_Random.NextFloat(0, 1));
                Particle.CreateParticle(ContentManager.LineParticle, pos, color, duration, 1f, state);
            }
        }

        #endregion
    }
}