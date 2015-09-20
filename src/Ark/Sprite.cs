#region Using Statements
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
#endregion

namespace Ark
{
    public abstract class Sprite
    {
        #region Protected Members

        protected Texture2D Texture;

        #endregion

        #region Public Members

        public Rectangle Source;

        public Vector2 Position;
        public Vector2 Origin;

        public Color Color;

        public SpriteEffects Effects;

        public Single Rotation;
        public Single Scale;
        public Single Depth;

        public bool IsAlive;

        #endregion

        #region Initialisation

        public Sprite()
        {
            this.Texture = null;

            Source = new Rectangle(0, 0, 0, 0);

            Position = Vector2.Zero;
            Origin = Vector2.Zero;

            Color = Color.White;

            Effects = SpriteEffects.None;

            Rotation = 0f;
            Scale = 1f;
            Depth = 0f;

            IsAlive = false;
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public abstract void Update(GameTime gameTime);

        #endregion

        #region Draw

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effects, Depth);
        }

        #endregion
    }
}