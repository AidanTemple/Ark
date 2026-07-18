#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class Player : Sprite
    {
        #region Private Members

        private Viewport m_Viewport;

        private Rectangle m_ViewportRect;
        private Rectangle m_BoundingRect;

        private List<WeaponSlot> m_WeaponSlots;

        private GamePadState m_PreviousGamePadState;

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

            m_WeaponSlots = new List<WeaponSlot>
            {
                new WeaponSlot(new LaserWeapon(), Buttons.A),
                new WeaponSlot(new RailgunWeapon(), Buttons.X),
                new WeaponSlot(new GravityBombWeapon(), Buttons.Y),
            };
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            UpdateGamePad(gameTime);

            Position.X = (int)MathHelper.Clamp(Position.X, m_Viewport.X + (Width / 2),
                m_Viewport.Width - (Width / 2));
            Position.Y = (int)MathHelper.Clamp(Position.Y, m_Viewport.Y + (Height / 2),
                m_Viewport.Height - (Height / 2));

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;
        }

        // Separate from Update(GameTime) because Sprite's Update signature is
        // fixed and has no way to carry the current enemy list, which weapons
        // need to resolve hits/effects against. Called explicitly by GameScene.
        public void UpdateWeapons(GameTime gameTime, List<Enemy> enemies)
        {
            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                slot.Weapon.Update(gameTime, m_ViewportRect, enemies);
            }
        }

        private void UpdateGamePad(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 thumbstick = gamePadState.ThumbSticks.Left;

            // Left stick Y is +1 up / -1 down; screen space Y grows downward, so negate.
            Position += new Vector2(thumbstick.X, -thumbstick.Y) * GameVariables.PlayerSpeed * deltaTime;

            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                if (gamePadState.IsButtonDown(slot.TriggerButton) && m_PreviousGamePadState.IsButtonUp(slot.TriggerButton))
                {
                    slot.Weapon.TryFire(Position);
                }
            }

            m_PreviousGamePadState = gamePadState;
        }

        #endregion

        #region Helper Methods

        private void PutInStartPosition()
        {
            Position = new Vector2(m_Viewport.Width / 2, m_Viewport.Height - Height);
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);

                foreach (WeaponSlot slot in m_WeaponSlots)
                {
                    slot.Weapon.Draw(spriteBatch);
                }
            }
        }

        #endregion
    }
}
