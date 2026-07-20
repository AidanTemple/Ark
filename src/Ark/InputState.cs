#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class InputState
    {
        #region Public Members

        public const int m_MaxInputs = 4;

        public readonly GamePadState[] m_CurrentGamePadStates;
        public readonly GamePadState[] m_PreviousGamePadStates;

        public readonly bool[] m_GamePadWasConnected;

        public KeyboardState m_CurrentKeyboardState;
        public KeyboardState m_PreviousKeyboardState;

        public TouchCollection m_TouchState;

        public readonly List<GestureSample> m_Gestures = new List<GestureSample>();

        #endregion

        #region Initialisation

        public InputState()
        {
            m_CurrentGamePadStates = new GamePadState[m_MaxInputs];
            m_PreviousGamePadStates = new GamePadState[m_MaxInputs];

            m_GamePadWasConnected = new bool[m_MaxInputs];
        }

        #endregion

        #region Update

        public void Update()
        {
            for (int i = 0; i < m_MaxInputs; i++)
            {
                m_PreviousGamePadStates[i] = m_CurrentGamePadStates[i];

                m_CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (m_CurrentGamePadStates[i].IsConnected)
                {
                    m_GamePadWasConnected[i] = true;
                }
            }

            m_PreviousKeyboardState = m_CurrentKeyboardState;
            m_CurrentKeyboardState = Keyboard.GetState();

            m_TouchState = TouchPanel.GetState();

            m_Gestures.Clear();

            while(TouchPanel.IsGestureAvailable)
            {
                m_Gestures.Add(TouchPanel.ReadGesture());
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (m_CurrentGamePadStates[i].IsButtonDown(button) &&
                    m_PreviousGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// The keyboard isn't tied to a PlayerIndex the way a gamepad is, so
        /// unlike IsNewButtonPress this doesn't report which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return m_CurrentKeyboardState.IsKeyDown(key) && m_PreviousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex))
            {
                return true;
            }

            if (IsNewKeyPress(Keys.Enter))
            {
                // Keyboard has no player slot of its own -- fall back the
                // same way GameScene.Reset() already does when nothing else
                // says who's controlling.
                playerIndex = controllingPlayer ?? PlayerIndex.One;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            return IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex) ||
                IsNewKeyPress(Keys.Up);
        }

        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex) ||
                IsNewKeyPress(Keys.Down);
        }

        #endregion
    }
}
