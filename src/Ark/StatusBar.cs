#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public class StatusBar
    {
        #region Private Members

        private Texture2D m_Texture;

        private float m_Percent;

        private Color m_Color;

        #endregion 

        #region Properties

        public float Percent
        {
            get { return m_Percent; }
            set { m_Percent = value; }
        }

        public Color Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        // Top-left corner of the bar on screen. Defaults to (0, 0), the
        // position the health bar has always drawn at.
        public int X { get; set; }
        public int Y { get; set; }

        #endregion

        #region Initialisation

        public StatusBar()
        {
            m_Texture = ContentManager.StatusBar;

            if (m_Texture != null)
            {
                Width = m_Texture.Width;
                Height = m_Texture.Height;
            }

            m_Color = new Color(60, 150, 0);
        }

        #endregion

        #region Update

        public void Update()
        {
            m_Percent = (int)MathHelper.Clamp(m_Percent, 0, 100);
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_Texture, new Rectangle(X, Y, Width, Height),
                new Rectangle(0, 5, Width, Height), Color.Red);

            spriteBatch.Draw(m_Texture, new Rectangle(X, Y, (int)(Width * ((double)m_Percent / 100)),
                Height), new Rectangle(0, 5, Width, 5), m_Color);
        }

        #endregion
    }
}