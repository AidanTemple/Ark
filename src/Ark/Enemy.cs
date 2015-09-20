#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public class Enemy : Sprite
    {
        #region Constants

        private const int m_MaxLasers = 6;

        private const float m_FireInterval = 0.25f;

        #endregion

        #region Private Members

        private Viewport m_Viewport;

        private Rectangle m_ViewportRect;
        private Rectangle m_BoundingRect;

        private Vector2 m_Center;

        private float m_Health;
        private float m_CurrentHealth;
        private float m_Radius;
        private float m_LaserTime;
        private float m_Speed;

        private Laser[] m_Lasers;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        public float CurrentHealth
        {
            get { return m_CurrentHealth; }
            set { m_CurrentHealth = value;  }
        }

        public Rectangle BoundingRect
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        public Laser[] Lasers
        {
            get { return m_Lasers; }
            set { m_Lasers = value; }
        }

        public bool IsDead
        {
            get { return m_CurrentHealth <= 0; }
        }

        #endregion

        #region Initialisation

        public Enemy(GraphicsDevice graphicsDevice, Vector2 position, float health, float speed)
        {
            m_Viewport = graphicsDevice.Viewport;

            m_ViewportRect = new Rectangle(m_Viewport.X, m_Viewport.Y,
                m_Viewport.Width, m_Viewport.Height);

            Texture = ContentManager.Enemy;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                Position = position;

                m_Center = new Vector2(Position.X + Width / 2, 
                    Position.Y + Height / 2);

                BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                    (int)Position.Y - (int)Origin.Y, Width, Height);

                m_Health = health;
                m_CurrentHealth = m_Health;
                m_Speed = speed;

                IsAlive = true;
            }

            m_Radius = 1000;

            m_Lasers = new Laser[m_MaxLasers];

            for (int i = 0; i < m_MaxLasers; i++)
            {
                m_Lasers[i] = new Laser();

                if (i % 2 == 0)
                {
                    m_Lasers[i].Position.X = this.Position.X - (Width / 2);
                    m_Lasers[i].Position.Y = this.Position.Y;
                }
                else
                {
                    m_Lasers[i].Position.X = this.Position.X + (Width / 2);
                    m_Lasers[i].Position.Y = this.Position.Y;
                }
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if(m_CurrentHealth <= 0)
            {
                IsAlive = false;
            }

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            UpdateMovement();
            UpdateLaser(gameTime);
        }

        private void UpdateMovement()
        {
            this.Position.Y += m_Speed;

            if(this.Position.Y > 800)
            {
                this.Position.Y = -30;
            }
        }

        private void UpdateLaser(GameTime gameTime)
        {
            m_LaserTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < m_Lasers.Length; i++)
            {
                m_Lasers[i].Update(gameTime);

                if(m_Lasers[i].IsAlive)
                {
                    if (!m_ViewportRect.Contains(new Point((int)m_Lasers[i].Position.X,
                        (int)m_Lasers[i].Position.Y)))
                    {
                        m_Lasers[i].IsAlive = false;
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                foreach (Laser laser in m_Lasers)
                {
                    laser.Draw(spriteBatch);
                }

                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);

                base.Draw(spriteBatch);
            }
        }

        #endregion

        #region Helper Methods

        public void FireLaser()
        {
            if (m_LaserTime >= m_FireInterval)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (!m_Lasers[i].IsAlive)
                    {
                        m_Lasers[i].IsAlive = true;
                        m_Lasers[i].Velocity = new Vector2(0, 10);

                        if (i == 1)
                        {
                            m_Lasers[i].Position = new Vector2(this.Position.X + 14, this.Position.Y + 10);
                        }
                        else
                        {
                            m_Lasers[i].Position = new Vector2(this.Position.X - 15, this.Position.Y + 3);
                        }

                        return;
                    }
                }

                m_LaserTime = 0;
            }
        }

        public bool IsInRange(Vector2 position)
        {
            return Vector2.Distance(m_Center, position) <= m_Radius;
        }

        #endregion
    }
}