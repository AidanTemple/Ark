#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
#endregion

namespace Ark
{
    class GameScene : Scene
    {
        #region Private Members

        private Background m_Background;
        private List<Player> m_Players;

        private WaveManager m_WaveManager;

        private StatusBar m_HealthBar;

        private Countdown m_Countdown;

        private bool m_ReturnToMenu;

        private Microsoft.Xna.Framework.Content.ContentManager m_Content;

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

            m_Content = new Microsoft.Xna.Framework.Content.ContentManager(SceneManager.Game.Services, "Content");

            ContentManager.LoadGame(m_Content);

            Reset();
        }

        private void Reset()
        {
            GameVariables.Score = 0;

            m_Background = new Background(SceneManager.GraphicsDevice);

            m_Countdown = new Countdown();

            // A single-element list for now -- Player itself no longer hardcodes
            // PlayerIndex.One (it listens to whichever controller is passed in),
            // so a second entry here is all a future local co-op player would need
            // at the input level. Death handling and the health bar below are still
            // single-player-shaped by design; see the comments at their call sites.
            m_Players = new List<Player> { new Player(SceneManager.GraphicsDevice, ControllingPlayer ?? PlayerIndex.One) };

            m_WaveManager = new WaveManager(SceneManager.GraphicsDevice, ContentManager.Enemy, 24);

            m_HealthBar = new StatusBar();
            m_HealthBar.Percent = m_Players[0].Health;

            // Keep the existing manager across resets instead of replacing it --
            // particles already age themselves out via Update(), and replacing
            // it here would silently drop any burst spawned earlier this same
            // frame (e.g. a weapon killing an enemy on the same frame the
            // player dies and triggers this Reset()).
            if (Particle == null)
            {
                Particle = new ParticleManager<ParticleState>(1024 * 20, ParticleState.Update);
            }
        }

        public override void UnloadContent()
        {
            m_Content.Unload();

            ContentManager.UnloadGame();

            // Particle is static, not instance-scoped, so it otherwise
            // survives across separate GameScene instances (e.g. exit to
            // menu and start a new game) -- Reset()'s "if (Particle == null)"
            // guard exists to avoid dropping an in-flight burst on a
            // same-instance death respawn, not to keep a manager alive whose
            // particles may be holding a Texture2D that was just disposed
            // above.
            Particle = null;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            if (m_ReturnToMenu && IsExiting && TransitionPosition >= 1f)
            {
                SceneManager.AddScene(new MenuScene(), ControllingPlayer);
                m_ReturnToMenu = false;
            }

            m_Background.Update(gameTime);
            m_Countdown.Update(gameTime);

            if (!m_Countdown.IsCountingDown)
            {
                HandleCollisions();

                foreach (Player player in m_Players)
                {
                    player.Update(gameTime);

                    // Separate call because weapons need the current enemy list to
                    // resolve hits/effects, and Sprite.Update's signature can't
                    // carry it -- keep this before WaveManager.Update so damage
                    // resolves against enemies' pre-movement positions this frame.
                    player.UpdateWeapons(gameTime, m_WaveManager.Enemies);
                }

                m_WaveManager.Update(gameTime);

                Particle.Update();

                // Health bar and the death check below are still tied to a single
                // player (m_Players[0]) -- a multi-player HUD and whether one co-op
                // player dying should end the round for everyone are game-design
                // decisions this pass doesn't answer, not architecture ones.
                m_HealthBar.Percent = m_Players[0].Health;
                m_HealthBar.Update();

                if (m_Players[0].Health <= 0 && m_Players[0].IsAlive)
                {
                    Reset();
                }

                foreach (Enemy enemy in m_WaveManager.Enemies)
                {
                    if (enemy.IsAlive)
                    {
                        foreach (Player player in m_Players)
                        {
                            if (player.IsAlive && enemy.IsOnScreen && enemy.IsInRange(player.Position))
                            {
                                if (player.Position.X > enemy.Position.X - enemy.Origin.X
                                    && player.Position.X < enemy.Position.X + enemy.Origin.X)
                                {
                                    enemy.FireLaser();
                                    break;
                                }
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
                    m_ReturnToMenu = true;
                    ExitScene();
                }
            }
        }

        private void HandleCollisions()
        {
            foreach (Enemy enemy in m_WaveManager.Enemies)
            {
                foreach (Laser laser in enemy.Lasers)
                {
                    if (!laser.IsAlive)
                    {
                        continue;
                    }

                    foreach (Player player in m_Players)
                    {
                        if (Physics.Overlaps(laser.BoundingRect, player.BoundingRect))
                        {
                            laser.IsAlive = false;
                            player.Health -= laser.Damage;

                            break;
                        }
                    }

                    if (!laser.IsAlive)
                    {
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

            foreach (Player player in m_Players)
            {
                player.Draw(SceneManager.SpriteBatch);
            }

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
    }
}