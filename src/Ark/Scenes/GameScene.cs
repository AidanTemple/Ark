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

                // No menu to return to -- Back/Escape exit the game outright.
                if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player) ||
                    input.IsNewKeyPress(Keys.Escape))
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

            DrawHud(spriteBatch);

            spriteBatch.End();

            if (TransitionPosition > 0)
            {
                SceneManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
            }
        }

        // Basic real-time readout of the player's systems and their current
        // usage, top-left corner -- HUD-only, so it's tied to m_Players[0]
        // like other UI-only code in this file rather than looping every
        // player.
        private void DrawHud(SpriteBatch spriteBatch)
        {
            if (ContentManager.HudFont == null || m_Players.Count == 0)
            {
                return;
            }

            Player player = m_Players[0];
            Vector2 position = new Vector2(10, 10);

            DrawHudLine(spriteBatch, $"HEALTH: {player.Health:0}", ref position);
            DrawHudLine(spriteBatch, $"SHIELD: {player.ShieldPercent:0}%", ref position);
            DrawHudLine(spriteBatch, $"ARMOR: {player.ArmorDamageReduction:0}", ref position);
            DrawHudLine(spriteBatch, $"CAPACITOR: {player.CapacitorPercent:0}%", ref position);
            DrawHudLine(spriteBatch, $"PROPULSION: x{player.PropulsionSpeedMultiplier:0.00}", ref position);

            for (int i = 0; i < player.Weapons.Count; i++)
            {
                Weapon weapon = player.Weapons[i];
                string status = weapon.IsCharging ? "CHARGING" : (weapon.IsReady ? "READY" : "COOLDOWN");

                // Keyboard fire (Space) always targets whichever slot 1/2/3
                // last selected -- marked here so pressing 1/2/3 has some
                // visible effect even before Space/a gamepad trigger
                // actually fires anything.
                string selected = i == player.SelectedWeaponIndex ? " [SELECTED]" : "";

                DrawHudLine(spriteBatch, $"{weapon.Name.ToUpperInvariant()}: {status}{selected}", ref position);
            }
        }

        private void DrawHudLine(SpriteBatch spriteBatch, string text, ref Vector2 position)
        {
            spriteBatch.DrawString(ContentManager.HudFont, text, position, Color.White);
            position.Y += ContentManager.HudFont.LineSpacing;
        }

        #endregion
    }
}
