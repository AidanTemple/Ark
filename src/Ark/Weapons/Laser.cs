#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public class Laser : Sprite
    {
        #region Private Members

        private Rectangle m_BoundingRect;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 Velocity { get; set; }

        public float Damage { get; set; }

        public Rectangle BoundingRect
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        #endregion

        #region Initialisation

        public Laser()
        {
            Texture = ContentManager.EnemyLaser;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                m_BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                    (int)Position.Y - (int)Origin.Y, Width, Height);

                IsAlive = false;
            }

            Damage = GameVariables.LaserDamage;
        }

        #endregion

        #region Update

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Position += Velocity;

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);

                base.Draw(spriteBatch);
            }
        }

        #endregion
    }
}