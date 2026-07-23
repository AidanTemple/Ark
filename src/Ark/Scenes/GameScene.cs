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

        private List<Player> m_Players;

        private Microsoft.Xna.Framework.Content.ContentManager m_Content;

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
            // A single-element list for now -- Player itself no longer hardcodes
            // PlayerIndex.One (it listens to whichever controller is passed in),
            // so a second entry here is all a future local co-op player would
            // need at the input level.
            m_Players = new List<Player> { new Player(SceneManager.GraphicsDevice, ControllingPlayer ?? PlayerIndex.One) };
        }

        public override void UnloadContent()
        {
            m_Content.Unload();

            ContentManager.UnloadGame();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            foreach (Player player in m_Players)
            {
                player.Update(gameTime);
            }
        }

        public override void UpdateInput(InputState input)
        {
            if(input != null)
            {
                PlayerIndex player;

                // No menu to return to -- Back exits the game outright.
                if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
                {
                    SceneManager.Game.Exit();
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            spriteBatch.Begin();

            foreach (Player player in m_Players)
            {
                player.Draw(SceneManager.SpriteBatch);
            }

            spriteBatch.End();

            if (TransitionPosition > 0)
            {
                SceneManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
            }
        }

        #endregion
    }
}
