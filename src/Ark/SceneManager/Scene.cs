#region Using Statements
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public enum SceneState
    {
        #region States

        TransitionOn,
        Active,
        TransitionOff,
        Hidden

        #endregion
    }

    public abstract class Scene
    {
        #region Private Members

        private bool m_IsPopup = false;
        private bool m_IsExiting = false;
        private bool m_HasFocus = false;
        private bool m_IsSerializable = true;

        private TimeSpan m_TransitionOnTime = TimeSpan.Zero;
        private TimeSpan m_TransitionOffTime = TimeSpan.Zero;

        private float m_TransitionPosition = 1;

        private SceneState m_State = SceneState.TransitionOn;
        private SceneManager m_Manager;

        private PlayerIndex? m_PlayerIndex;

        private GestureType m_EnabledGestures = GestureType.None;

        #endregion

        #region Properties

        public bool IsPopup
        {
            get { return m_IsPopup; }
            set { m_IsPopup = value; }
        }

        public bool IsExiting
        {
            get { return m_IsExiting; }
            set { m_IsExiting = value; }
        }

        public bool IsActive
        {
            get
            {
                return !m_HasFocus && (m_State == SceneState.TransitionOn
                    || m_State == SceneState.Active);
            }
        }

        public TimeSpan TransitionOnTime
        {
            get { return m_TransitionOnTime; }
            set { m_TransitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return m_TransitionOffTime; }
            set { m_TransitionOffTime = value; }
        }

        public float TransitionPosition
        {
            get { return m_TransitionPosition; }
            set { m_TransitionPosition = value; }
        }

        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        public SceneState SceneState
        {
            get { return m_State; }
            protected set { m_State = value; }
        }

        public SceneManager SceneManager
        {
            get { return m_Manager; }
            internal set { m_Manager = value; }
        }

        public PlayerIndex? ControllingPlayer
        {
            get { return m_PlayerIndex; }
            internal set { m_PlayerIndex = value; }
        }

        public GestureType EnabledGestures
        {
            get { return m_EnabledGestures; }
            protected set
            {
                m_EnabledGestures = value;

                if (m_State == SceneState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        public bool IsSerializable
        {
            get { return m_IsSerializable; }
            set { m_IsSerializable = value; }
        }

        #endregion

        #region Initialisation

        public virtual void LoadContent()
        {

        }

        public virtual void UnloadContent()
        {

        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime, bool hasFocus, bool coveredByOtherScreen)
        {
            this.m_HasFocus = hasFocus;

            if (m_IsExiting)
            {
                m_State = SceneState.TransitionOff;

                if (!UpdateTransition(gameTime, m_TransitionOffTime, 1))
                {
                    SceneManager.RemoveScene(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                if (UpdateTransition(gameTime, m_TransitionOffTime, 1))
                {
                    m_State = SceneState.TransitionOff;
                }
                else
                {
                    m_State = SceneState.Hidden;
                }
            }
            else
            {
                if (UpdateTransition(gameTime, m_TransitionOnTime, -1))
                {
                    m_State = SceneState.TransitionOn;
                }
                else
                {
                    m_State = SceneState.Active;
                }
            }
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float transitionDelta;

            if (time == TimeSpan.Zero)
            {
                transitionDelta = 1;
            }
            else
            {
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds
                    / time.TotalMilliseconds);
            }

            m_TransitionPosition += transitionDelta * direction;

            if (((direction < 0) && (m_TransitionPosition <= 0)) ||
                ((direction > 0) && (m_TransitionPosition >= 1)))
            {
                m_TransitionPosition = MathHelper.Clamp(m_TransitionPosition, 0, 1);
                return false;
            }

            return true;
        }

        public virtual void UpdateInput(InputState input)
        {

        }

        #endregion

        #region Draw

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }

        #endregion

        #region Helper Methods

        public virtual void Serialize(Stream stream)
        {

        }

        public virtual void Deserialize(Stream stream)
        {

        }

        public void ExitScene()
        {
            if(TransitionOffTime == TimeSpan.Zero)
            {
                SceneManager.RemoveScene(this);
            }
            else
            {
                m_IsExiting = true;
            }
        }

        #endregion
    }
}
