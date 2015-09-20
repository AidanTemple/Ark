#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Ark
{
    public class Background : Sprite
    {
        #region Private Members

        private Viewport m_Viewport;

        private Vector2 m_Size;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        #endregion

        #region Initialisation

        public Background(GraphicsDevice graphicsDevice)
        {
            m_Viewport = graphicsDevice.Viewport;

            Texture = ContentManager.Background_001;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;
            }

            Origin = new Vector2(Width / 2, 0);

            Position = new Vector2(m_Viewport.Width / 2, 
                m_Viewport.Height / 2);

            m_Size = new Vector2(0, Height);

            IsAlive = true;
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            float deltaY = (float)gameTime.ElapsedGameTime.TotalSeconds * GameVariables.BackgroundScrollSpeed;

            Position.Y += deltaY;
            Position.Y = Position.Y % Height;
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                if (Position.Y < m_Viewport.Height)
                {
                    spriteBatch.Draw(Texture, Position, null, Color.White, 
                        Rotation, Origin, Scale, SpriteEffects.None, Depth);
                }

                spriteBatch.Draw(Texture, Position - m_Size, null, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
        }

        #endregion
    }
}