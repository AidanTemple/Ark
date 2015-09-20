#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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
            EnabledGestures = GestureType.Tap;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0);

            m_MenuBackground = ContentManager.Background_004;
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

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.End();
        }

        #endregion

        #region Helper Methods

        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            return new Rectangle(0, (int)entry.Position.Y - m_Padding,
                SceneManager.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this) + (m_Padding * 2));
        }

        public override void UpdateInput(InputState input)
        {
            PlayerIndex player;

            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }

            foreach(GestureSample gesture in input.m_Gestures)
            {
                if(gesture.GestureType == GestureType.Tap)
                {
                    Point tap = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    for(int i = 0; i < m_Entries.Count; i++)
                    {
                        MenuEntry entry = MenuEntries[i];

                        if(GetMenuEntryHitBounds(entry).Contains(tap))
                        {
                            OnSelectEntry(i, PlayerIndex.One);
                        }
                    }
                }
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
