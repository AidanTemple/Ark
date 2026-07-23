#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class Player : Ship
    {
        #region Private Members

        private List<WeaponSlot> m_WeaponSlots;

        private GamePadState m_PreviousGamePadState;
        private KeyboardState m_PreviousKeyboardState;
        private MouseState m_PreviousMouseState;

        // Point-to-move helm control: a shared on-screen reticle steered by
        // either the mouse or the gamepad's left stick; a click/A-press
        // locks its current position in as the ship's destination (see
        // Ship.SetDestination).
        private Vector2 m_Cursor;

        #endregion

        #region Properties

        // Named Index, not PlayerIndex, so it doesn't shadow the
        // PlayerIndex enum type within this class.
        public PlayerIndex Index { get; private set; }

        // Which weapon slot Space fires -- the gamepad has 3 independent
        // triggers (B/X/Y), but the keyboard only has one fire key, so 1/2/3
        // pick which slot Space targets. Index order matches m_WeaponSlots
        // (and Ship.Weapons, since the constructor adds them in the same
        // order) -- Laser/Railgun/GravityBomb. Public so the HUD can show
        // which weapon 1/2/3 last selected.
        public int SelectedWeaponIndex { get; private set; }

        #endregion

        #region Initialisation

        public Player(GraphicsDevice graphicsDevice, PlayerIndex index)
            : base(graphicsDevice, ContentManager.Player)
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

            if (Texture != null)
            {
                m_Cursor = Position;
            }

            // Laser moved off A (its default elsewhere in this project's
            // history) so A is free to mean "confirm the cursor's position
            // as the ship's destination" -- B is otherwise unused during
            // gameplay.
            m_WeaponSlots = new List<WeaponSlot>
            {
                new WeaponSlot(new LaserWeapon(), Buttons.B),
                new WeaponSlot(new RailgunWeapon(), Buttons.X),
                new WeaponSlot(new GravityBombWeapon(), Buttons.Y),
            };

            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                m_Weapons.Add(slot.Weapon);
            }
        }

        // One of each non-weapon slot fitted, for now -- see Ship's
        // Create*Module factory methods.
        protected override ShieldModule CreateShieldModule() => new ShieldModule();
        protected override ArmorModule CreateArmorModule() => new ArmorModule();
        protected override CapacitorModule CreateCapacitorModule() => new CapacitorModule();
        protected override PropulsionModule CreatePropulsionModule() => new PropulsionModule();

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            // Polled once and shared -- UpdateCursor and UpdateGamePad both
            // need this frame's gamepad state, and re-polling per call is a
            // redundant hardware read for the exact same instant.
            GamePadState gamePadState = GamePad.GetState(Index);

            // Input handling (including firing) runs before base.Update --
            // base.Update() is what steers toward whatever destination
            // UpdateCursor just set, and updates every weapon's projectiles
            // by one frame, so a shot fired this frame should already be
            // queued up before that loop runs.
            UpdateCursor(gameTime, gamePadState);
            UpdateGamePad(gamePadState);
            UpdateKeyboard(gameTime);

            base.Update(gameTime);
        }

        // Point-to-move targeting: the mouse and the gamepad's left stick
        // share one on-screen cursor. The mouse is authoritative the
        // instant its OS position changes; otherwise the stick is free to
        // nudge the cursor -- so the two input methods don't fight over it
        // every frame.
        private void UpdateCursor(GameTime gameTime, GamePadState gamePadState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();

            if (mouseState.Position != m_PreviousMouseState.Position)
            {
                m_Cursor = new Vector2(mouseState.Position.X, mouseState.Position.Y);

                // The OS cursor can sit outside the window's client area
                // (e.g. near a window edge) -- clamp the same way the
                // gamepad-stick path below already does, so a click at
                // that instant can't set a destination outside the
                // viewport (the ship would otherwise never arrive).
                m_Cursor = Physics.ClampToBounds(m_Cursor, m_ViewportRect, 0, 0);
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
                SetDestination(m_Cursor);
            }

            if (gamePadState.IsButtonDown(Buttons.A) && m_PreviousGamePadState.IsButtonUp(Buttons.A))
            {
                SetDestination(m_Cursor);
            }

            m_PreviousMouseState = mouseState;
        }

        private void UpdateGamePad(GamePadState gamePadState)
        {
            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                if (gamePadState.IsButtonDown(slot.TriggerButton) && m_PreviousGamePadState.IsButtonUp(slot.TriggerButton))
                {
                    slot.Weapon.TryFire(Position, Capacitor);
                }
            }

            m_PreviousGamePadState = gamePadState;
        }

        private void UpdateKeyboard(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsNewKeyPress(keyboardState, Keys.D1))
            {
                SelectedWeaponIndex = 0;
            }
            else if (IsNewKeyPress(keyboardState, Keys.D2))
            {
                SelectedWeaponIndex = 1;
            }
            else if (IsNewKeyPress(keyboardState, Keys.D3))
            {
                SelectedWeaponIndex = 2;
            }

            if (IsNewKeyPress(keyboardState, Keys.Space))
            {
                m_WeaponSlots[SelectedWeaponIndex].Weapon.TryFire(Position, Capacitor);
            }

            m_PreviousKeyboardState = keyboardState;
        }

        private bool IsNewKeyPress(KeyboardState keyboardState, Keys key)
        {
            return keyboardState.IsKeyDown(key) && m_PreviousKeyboardState.IsKeyUp(key);
        }

        #endregion
    }
}
