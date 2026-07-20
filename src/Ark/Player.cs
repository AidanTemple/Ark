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
        private ShieldSlot m_ShieldSlot;

        private GamePadState m_PreviousGamePadState;

        #endregion

        #region Properties

        // Named Index, not PlayerIndex, so it doesn't shadow the
        // PlayerIndex enum type within this class.
        public PlayerIndex Index { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public float Health { get; set; }

        public float ShieldPercent
        {
            get { return m_ShieldSlot.Shield.Percent; }
        }

        private Vector2 Center { get; set; }

        public Rectangle BoundingRect
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        #endregion

        #region Initialisation

        public Player(GraphicsDevice graphicsDevice, PlayerIndex index)
        {
            Index = index;

            // Seed with a real poll instead of the struct default (all buttons
            // up) -- otherwise a button already held on the frame this Player
            // is constructed (e.g. still holding A from selecting "New Game")
            // reads as a fresh press on the very first UpdateGamePad call.
            m_PreviousGamePadState = GamePad.GetState(Index);

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

            // Default loadout is Basic -- swap to a higher tier later via
            // m_ShieldSlot.Equip(new Shield(ShieldTier.Advanced)) etc.
            m_ShieldSlot = new ShieldSlot(new Shield(ShieldTier.Basic));
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            UpdateGamePad(gameTime);

            Position = Physics.ClampToBounds(Position, m_ViewportRect, Width / 2, Height / 2);

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            m_ShieldSlot.Shield.Update(gameTime);
        }

        // Separate from Update(GameTime) because Sprite's Update signature is
        // fixed and has no way to carry the current enemy/asteroid lists,
        // which weapons need to resolve hits/effects against. Called
        // explicitly by GameScene.
        public void UpdateWeapons(GameTime gameTime, List<Enemy> enemies, List<Asteroid> asteroids)
        {
            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                slot.Weapon.Update(gameTime, m_ViewportRect, enemies, asteroids);
            }
        }

        private void UpdateGamePad(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(Index);

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

        // The single entry point for anything hurting the ship -- the shield
        // soaks first, and Health is only reduced by whatever gets past it.
        // Callers must not subtract Health directly, or the shield (and its
        // repair-delay timer) is silently bypassed.
        public void TakeDamage(float damage)
        {
            damage = m_ShieldSlot.Shield.Absorb(damage);

            if (damage > 0)
            {
                Health -= damage;
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

                foreach (WeaponSlot slot in m_WeaponSlots)
                {
                    slot.Weapon.Draw(spriteBatch);
                }
            }
        }

        #endregion
    }
}
