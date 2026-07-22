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
        private KeyboardState m_PreviousKeyboardState;
        private MouseState m_PreviousMouseState;

        // Which weapon slot Space fires -- the gamepad has 3 independent
        // triggers (B/X/Y), but the keyboard only has one fire key, so 1/2/3
        // pick which slot Space targets. Index order matches the slot list
        // below (Laser/Railgun/GravityBomb).
        private int m_SelectedWeaponIndex;

        // Point-to-move helm control: m_Cursor is a shared on-screen
        // reticle steered by either the mouse or the gamepad's left stick;
        // a click/A-press locks its current position in as m_Destination,
        // which the ship then turns to face and accelerates toward.
        private Vector2 m_Cursor;
        private Vector2 m_Destination;
        private Vector2 m_Velocity;

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

            // Same reasoning as the gamepad seed above -- a key already held
            // this frame (e.g. Enter, still down from selecting "New Game")
            // shouldn't read as a fresh press on the first UpdateKeyboard call.
            m_PreviousKeyboardState = Keyboard.GetState();

            // Same reasoning again -- without this, frame one would almost
            // always read as "the mouse just moved" (comparing against the
            // struct default position of (0,0)), snapping m_Cursor away
            // from the ship before the player has done anything.
            m_PreviousMouseState = Mouse.GetState();

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

                // Start already "arrived" at spawn -- otherwise the ship
                // would immediately steer toward Vector2.Zero (top-left).
                m_Destination = Position;
                m_Cursor = Position;

                IsAlive = true;
            }

            // Laser moved off A (its default elsewhere in this project's
            // history) so A is free to mean "confirm the cursor's position
            // as the ship's destination" -- B is otherwise unused during
            // gameplay (only relevant to menu-cancel in Menu/MenuScene).
            m_WeaponSlots = new List<WeaponSlot>
            {
                new WeaponSlot(new LaserWeapon(), Buttons.B),
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
            UpdateCursor(gameTime);
            UpdateSteering(gameTime);
            UpdateGamePad(gameTime);
            UpdateKeyboard(gameTime);

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

        // Point-to-move targeting: the mouse and the gamepad's left stick
        // share one on-screen cursor. The mouse is authoritative the
        // instant its OS position changes; otherwise the stick is free to
        // nudge the cursor -- so the two input methods don't fight over it
        // every frame.
        private void UpdateCursor(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();
            GamePadState gamePadState = GamePad.GetState(Index);

            if (mouseState.Position != m_PreviousMouseState.Position)
            {
                m_Cursor = new Vector2(mouseState.Position.X, mouseState.Position.Y);
            }
            else
            {
                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                if (thumbstick != Vector2.Zero)
                {
                    // Left stick Y is +1 up / -1 down; screen space Y grows
                    // downward, so negate.
                    m_Cursor += new Vector2(thumbstick.X, -thumbstick.Y) * GameVariables.CursorSpeed * deltaTime;
                    m_Cursor = Physics.ClampToBounds(m_Cursor, m_ViewportRect, 0, 0);
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed && m_PreviousMouseState.LeftButton == ButtonState.Released)
            {
                m_Destination = m_Cursor;
            }

            if (gamePadState.IsButtonDown(Buttons.A) && m_PreviousGamePadState.IsButtonUp(Buttons.A))
            {
                m_Destination = m_Cursor;
            }

            m_PreviousMouseState = mouseState;
        }

        // Ship-like steering toward m_Destination: turns to face it at a
        // limited rate and eases into/out of motion with real acceleration
        // and drag, rather than snapping directly to an input direction.
        private void UpdateSteering(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 toDestination = m_Destination - Position;
            float distance = toDestination.Length();

            if (distance > GameVariables.PlayerArrivalRadius)
            {
                float desiredHeading = toDestination.ToAngle();
                float turnDelta = MathHelper.WrapAngle(desiredHeading - Rotation);
                float maxTurn = MathHelper.ToRadians(GameVariables.PlayerTurnRateDegrees) * deltaTime;

                Rotation = MathHelper.WrapAngle(Rotation + MathHelper.Clamp(turnDelta, -maxTurn, maxTurn));

                // Arrive behavior: ease off the desired speed as it nears
                // the destination so it settles in instead of overshooting
                // and oscillating around it.
                float speedFactor = MathHelper.Clamp(distance / GameVariables.PlayerSlowRadius, 0f, 1f);

                Vector2 desiredVelocity = toDestination;
                desiredVelocity.Normalize();
                desiredVelocity *= GameVariables.PlayerSpeed * speedFactor;

                Vector2 steering = desiredVelocity - m_Velocity;
                float maxAccel = GameVariables.PlayerAcceleration * deltaTime;

                if (steering.Length() > maxAccel)
                {
                    steering.Normalize();
                    steering *= maxAccel;
                }

                m_Velocity += steering;
            }
            else
            {
                m_Velocity *= GameVariables.PlayerDragFactor;
            }

            Position += m_Velocity * deltaTime;
        }

        private void UpdateGamePad(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(Index);

            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                if (gamePadState.IsButtonDown(slot.TriggerButton) && m_PreviousGamePadState.IsButtonUp(slot.TriggerButton))
                {
                    slot.Weapon.TryFire(Position);
                }
            }

            m_PreviousGamePadState = gamePadState;
        }

        private void UpdateKeyboard(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsNewKeyPress(keyboardState, Keys.D1))
            {
                m_SelectedWeaponIndex = 0;
            }
            else if (IsNewKeyPress(keyboardState, Keys.D2))
            {
                m_SelectedWeaponIndex = 1;
            }
            else if (IsNewKeyPress(keyboardState, Keys.D3))
            {
                m_SelectedWeaponIndex = 2;
            }

            if (IsNewKeyPress(keyboardState, Keys.Space))
            {
                m_WeaponSlots[m_SelectedWeaponIndex].Weapon.TryFire(Position);
            }

            m_PreviousKeyboardState = keyboardState;
        }

        private bool IsNewKeyPress(KeyboardState keyboardState, Keys key)
        {
            return keyboardState.IsKeyDown(key) && m_PreviousKeyboardState.IsKeyUp(key);
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

        // The evadable subset of this ship's live projectiles (Gravity
        // Bomb excluded -- see Weapon.IsEvadable) for enemies to react to.
        public List<Projectile> GetEvadableProjectiles()
        {
            List<Projectile> projectiles = new List<Projectile>();

            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                if (!slot.Weapon.IsEvadable)
                {
                    continue;
                }

                foreach (Projectile projectile in slot.Weapon.Projectiles)
                {
                    if (projectile.IsAlive)
                    {
                        projectiles.Add(projectile);
                    }
                }
            }

            return projectiles;
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                spriteBatch.DrawSafe(Texture, Position, null, Color.White, Rotation,
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
