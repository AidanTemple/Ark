#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public class Projectile : Sprite
    {
        #region Properties

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Vector2 Velocity { get; set; }

        public Rectangle BoundingRect { get; protected set; }

        #endregion

        #region Initialisation

        public Projectile(Texture2D texture)
        {
            Texture = texture;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                RecomputeBoundingRect();

                IsAlive = false;
            }
        }

        #endregion

        #region Update

        public virtual void Activate(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;

            RecomputeBoundingRect();

            IsAlive = true;
        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity;

            RecomputeBoundingRect();
        }

        protected void RecomputeBoundingRect()
        {
            BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                (int)Position.Y - (int)Origin.Y, Width, Height);
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.DrawSafe(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);
            }
        }

        #endregion
    }
}
