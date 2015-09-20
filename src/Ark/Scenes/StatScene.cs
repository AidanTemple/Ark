#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace Ark
{
    class StatScene : Scene
    {
        #region Private Members

        private Texture2D m_Texture;

        #endregion

        #region Initialisation

        public StatScene()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);
        }

        public override void LoadContent()
        {
            m_Texture = ContentManager.StatTexture;
        }

        public override void UnloadContent()
        {
            
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, hasFocus, coveredByOtherScreen);
        }

        public override void UpdateInput(InputState input)
        {
            if(input != null)
            {
                int playerIndex = (int)ControllingPlayer.Value;

                KeyboardState keyboardState = input.m_CurrentKeyboardStates[playerIndex];
                GamePadState gamePadState = input.m_CurrentGamePadStates[playerIndex];

                PlayerIndex player;

                if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
                {
                    LoadScene.Load(SceneManager, false, ControllingPlayer, new MenuScene());
                }
            }
        }
        
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            spriteBatch.Begin();

            spriteBatch.Draw(m_Texture, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}