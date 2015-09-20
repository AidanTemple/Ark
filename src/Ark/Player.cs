#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace Ark
{
    public class Player : Sprite
    {
        #region Constants

        private const int m_MaxMissiles = 3;

        #endregion

        #region Private Members

        private Viewport m_Viewport;

        private Rectangle m_ViewportRect;
        private Rectangle m_BoundingRect;

        private Missile[] m_Missiles;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        public float Health { get; set; }

        private Vector2 Center { get; set; }

        public Rectangle BoundingRect 
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        public Missile[] Missiles
        {
            get { return m_Missiles; }
            set { m_Missiles = value; }
        }

        #endregion

        #region Initialisation

        public Player(GraphicsDevice graphicsDevice)
        {
            m_Viewport = graphicsDevice.Viewport;

            m_ViewportRect = new Rectangle(m_Viewport.X, m_Viewport.Y,
                m_Viewport.Width, m_Viewport.Height);

            Texture = ContentManager.Player;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                Center = new Vector2(Position.X + Width / 2,
                    Position.Y - Height / 2);

                BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                    (int)Position.Y - (int)Origin.Y, Width, Height);

                Health = 100;

                PutInStartPosition();

                IsAlive = true;
            }

            m_Missiles = new Missile[m_MaxMissiles];

            for (int i = 0; i < m_MaxMissiles; i++)
            {
                m_Missiles[i] = new Missile();
                m_Missiles[i].Position = this.Position;
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            Position.X = (int)MathHelper.Clamp(Position.X, m_Viewport.X + (Width / 2), 
                m_Viewport.Width - (Width / 2));

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            UpdateMissiles(gameTime);
        }

        private void UpdateMissiles(GameTime gameTime)
        {
            foreach (Missile missile in m_Missiles)
            {
                missile.Update(gameTime);

                if (missile.IsAlive)
                {
                    if (!m_ViewportRect.Contains(new Point((int)missile.Position.X,
                        (int)missile.Position.Y)))
                    {
                        missile.IsAlive = false;
                        continue;
                    }
                }
            }
        }

        public void UpdateInput(InputState input)
        {
            foreach (GestureSample gesture in input.m_Gestures)
            {
                switch (gesture.GestureType)
                {
                    case GestureType.FreeDrag:
                        Position += gesture.Delta;
                        break;

                    case GestureType.Tap:
                        FireMissile();
                        break;
                }
            }
        }

        #endregion

        #region Helper Methods

        private void PutInStartPosition()
        {
            Position = new Vector2(m_Viewport.Width / 2, m_Viewport.Height - Height);
        }

        private void FireMissile()
        {
            foreach (Missile missile in m_Missiles)
            {
                if (!missile.IsAlive)
                {
                    missile.IsAlive = true;
                    missile.Position = new Vector2(this.Position.X,
                        this.Position.Y);
                    missile.Velocity = new Vector2(0, -10);

                    return;
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, 
                    Origin, Scale, SpriteEffects.None, Depth);

                foreach (Missile missile in m_Missiles)
                    missile.Draw(spriteBatch);

                base.Draw(spriteBatch);
            }
        }

        #endregion
    }
}