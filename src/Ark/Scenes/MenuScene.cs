#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    class MenuScene : Menu
    {
        #region Initialisation

        public MenuScene()
            : base()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            MenuEntry GameMenuEntry = new MenuEntry("New Game");
            MenuEntry ControlMenuEntry = new MenuEntry("Controls");
            MenuEntry ExitMenuEntry = new MenuEntry("Exit");

            GameMenuEntry.Selected += GameMenuEntrySelected;
            ControlMenuEntry.Selected += ControlMenuEntrySelected;
            ExitMenuEntry.Selected += ExitMenuEntrySelected;

            MenuEntries.Add(GameMenuEntry);
            MenuEntries.Add(ControlMenuEntry);
            MenuEntries.Add(ExitMenuEntry);
        }

        #endregion

        #region Event Handlers

        private void GameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadScene.Load(SceneManager, true, e.PlayerIndex, new GameScene());
        }

        private void ControlMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadScene.Load(SceneManager, false, e.PlayerIndex, new ControlScene());
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
