#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
#endregion

namespace Ark
{
    abstract class Menu : Scene
    {
        #region Private Members

        private const int m_Padding = 10;

        private List<MenuEntry> m_Entries = new List<MenuEntry>();
        private int m_Index = 0;

        private Texture2D m_MenuBackground;

        private Microsoft.Xna.Framework.Content.ContentManager m_Content;

        #endregion

        #region Properties

        protected IList<MenuEntry>MenuEntries
        {
            get { return m_Entries; }
        }

        #endregion

        #region Initialisation

        public Menu()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0);
        }

        public override void LoadContent()
        {
            m_Content = new Microsoft.Xna.Framework.Content.ContentManager(SceneManager.Game.Services, "Content");

            ContentManager.LoadMenu(m_Content);

            m_MenuBackground = ContentManager.MenuBackground;
        }

        public override void UnloadContent()
        {
            m_Content.Unload();

            ContentManager.UnloadMenu();
        }

        #endregion

        #region Update

        protected virtual void UpdateMenyEntryLocation()
        {
            Vector2 position = new Vector2(0f, 320f);

            for(int i = 0; i < m_Entries.Count; i++)
            {
                MenuEntry entry = MenuEntries[i];

                position.X = SceneManager.GraphicsDevice.Viewport.Width / 2 - entry.GetWidth(this) / 2;

                entry.Position = position;
                position.Y += entry.GetHeight(this) + (m_Padding * 4);
            }
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);

            for(int i = 0; i < m_Entries.Count; i++)
            {
                bool selected = IsActive && (i == m_Index);
                m_Entries[i].Update(this, selected, gameTime);
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            UpdateMenyEntryLocation();

            GraphicsDevice graphics = SceneManager.GraphicsDevice;
            SpriteFont font = SceneManager.MenuFont;

            spriteBatch.Begin();

            spriteBatch.Draw(m_MenuBackground, Vector2.Zero, Color.White);

            for (int i = 0; i < m_Entries.Count; i++)
            {
                MenuEntry entry = m_Entries[i];

                bool selected = IsActive && (i == m_Index);

                entry.Draw(this, selected, gameTime);
            }

            spriteBatch.End();

            if (TransitionPosition > 0)
            {
                SceneManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);
            }
        }

        #endregion

        #region Helper Methods

        public override void UpdateInput(InputState input)
        {
            if (m_Entries.Count == 0)
                return;

            if (input.IsMenuUp(ControllingPlayer))
            {
                m_Index--;

                if (m_Index < 0)
                    m_Index = m_Entries.Count - 1;
            }
            else if (input.IsMenuDown(ControllingPlayer))
            {
                m_Index++;

                if (m_Index >= m_Entries.Count)
                    m_Index = 0;
            }

            PlayerIndex player;

            if (input.IsMenuSelect(ControllingPlayer, out player))
            {
                OnSelectEntry(m_Index, player);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out player))
            {
                OnCancel(player);
            }
        }

        protected virtual void OnSelectEntry(int index, PlayerIndex playerIndex)
        {
            m_Entries[index].OnSelectedEntry(playerIndex);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScene();
        }

        #endregion
    }
}
