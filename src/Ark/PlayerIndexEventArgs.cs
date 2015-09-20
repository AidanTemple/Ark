#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    class PlayerIndexEventArgs : EventArgs
    {
        #region Private Members

        private PlayerIndex m_PlayerIndex;

        #endregion

        #region Properties

        public PlayerIndex PlayerIndex
        {
            get { return m_PlayerIndex; }
        }

        #endregion

        #region Initialisation

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.m_PlayerIndex = playerIndex;
        }

        #endregion
    }
}
