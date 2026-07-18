#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class MenuScene : Menu
    {
        #region Private Members

        private Scene m_PendingScene;
        private PlayerIndex? m_PendingPlayer;

        #endregion

        #region Initialisation

        public MenuScene()
            : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);

            MenuEntry GameMenuEntry = new MenuEntry("New Game");
            MenuEntry ExitMenuEntry = new MenuEntry("Exit");

            GameMenuEntry.Selected += GameMenuEntrySelected;
            ExitMenuEntry.Selected += ExitMenuEntrySelected;

            MenuEntries.Add(GameMenuEntry);
            MenuEntries.Add(ExitMenuEntry);
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            if (m_PendingScene != null && IsExiting && TransitionPosition >= 1f)
            {
                SceneManager.AddScene(m_PendingScene, m_PendingPlayer);
                m_PendingScene = null;
            }
        }

        #endregion

        #region Event Handlers

        private void GameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            m_PendingScene = new GameScene();
            m_PendingPlayer = e.PlayerIndex;

            ExitScene();
        }

        private void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SceneManager.Game.Exit();
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            SceneManager.Game.Exit();
        }

        #endregion
    }
}
