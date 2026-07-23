#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

        // 0 (idle) to 1 (full thrust) -- ramps gradually rather than
        // snapping, see UpdateSteering.
        private float m_EnginePower;

        #endregion

        #region Properties

        // Named Index, not PlayerIndex, so it doesn't shadow the
        // PlayerIndex enum type within this class.
        public PlayerIndex Index { get; private set; }

        public int Width { get; set; }
        public int Height { get; set; }

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
            // gameplay.
            m_WeaponSlots = new List<WeaponSlot>
            {
                new WeaponSlot(new LaserWeapon(), Buttons.B),
                new WeaponSlot(new RailgunWeapon(), Buttons.X),
                new WeaponSlot(new GravityBombWeapon(), Buttons.Y),
            };
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            // Polled once and shared -- UpdateCursor and UpdateGamePad both
            // need this frame's gamepad state, and re-polling per call is a
            // redundant hardware read for the exact same instant.
            GamePadState gamePadState = GamePad.GetState(Index);

            UpdateCursor(gameTime, gamePadState);
            UpdateSteering(gameTime);
            UpdateGamePad(gamePadState);
            UpdateKeyboard(gameTime);

            Position = Physics.ClampToBounds(Position, m_ViewportRect, Width / 2, Height / 2);

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            foreach (WeaponSlot slot in m_WeaponSlots)
            {
                slot.Weapon.Update(gameTime, m_ViewportRect);
            }
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
                m_Destination = m_Cursor;
            }

            if (gamePadState.IsButtonDown(Buttons.A) && m_PreviousGamePadState.IsButtonUp(Buttons.A))
            {
                m_Destination = m_Cursor;
            }

            m_PreviousMouseState = mouseState;
        }

        // Ship-like steering toward m_Destination, with rotation and thrust
        // mutually exclusive -- the ship never turns while it still has
        // meaningful velocity, and never thrusts until it's pointed the
        // right way (within PlayerHeadingToleranceDegrees). Redirecting
        // mid-flight to a new destination therefore plays out in three
        // phases every real vessel goes through: brake off the old
        // heading's velocity, rotate in place once nearly stopped, then
        // accelerate along the new heading. This is a standard pattern for
        // non-arcade ship autopilots (e.g. EVE Online's align-then-approach
        // behavior) -- not a strafing Asteroids-style ship that can thrust
        // in any direction regardless of facing.
        private void UpdateSteering(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 toDestination = m_Destination - Position;
            float distance = toDestination.Length();

            if (distance <= GameVariables.PlayerArrivalRadius)
            {
                // Arrived -- nothing left to point at, just coast to a stop.
                SpoolDown(deltaTime);
            }
            else
            {
                float desiredHeading = toDestination.ToAngle();
                float wrappedDiff = MathHelper.WrapAngle(desiredHeading - Rotation);
                float toleranceRadians = MathHelper.ToRadians(GameVariables.PlayerHeadingToleranceDegrees);

                if (Math.Abs(wrappedDiff) > toleranceRadians)
                {
                    // Not pointed the right way. Only start turning once
                    // any existing velocity has bled off below the braking
                    // threshold -- otherwise keep braking instead.
                    bool isStopped = m_Velocity.LengthSquared() <=
                        GameVariables.PlayerBrakingSpeedThreshold * GameVariables.PlayerBrakingSpeedThreshold;

                    if (isStopped)
                    {
                        float maxTurn = MathHelper.ToRadians(GameVariables.PlayerTurnRateDegrees) * deltaTime;
                        float turnDelta = MathHelper.Clamp(wrappedDiff, -maxTurn, maxTurn);

                        Rotation = MathHelper.WrapAngle(Rotation + turnDelta);
                    }

                    SpoolDown(deltaTime);
                }
                else
                {
                    // Heading matches -- clear to thrust straight ahead.
                    // Arrive behavior (speedFactor) bleeds the speed target
                    // down near the destination so it settles in instead of
                    // overshooting.
                    m_EnginePower = MathHelper.Min(m_EnginePower + deltaTime / GameVariables.PlayerEngineSpoolUpTime, 1f);

                    float speedFactor = MathHelper.Clamp(distance / GameVariables.PlayerSlowRadius, 0f, 1f);
                    Vector2 forward = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

                    Steer(forward * GameVariables.PlayerSpeed * speedFactor * m_EnginePower, deltaTime);
                }
            }

            Position += m_Velocity * deltaTime;
        }

        // Shared by both the "arrived" and "not pointed the right way"
        // cases in UpdateSteering -- brakes toward a stop and lets the
        // engine spool back down toward idle.
        private void SpoolDown(float deltaTime)
        {
            Brake(deltaTime);
            m_EnginePower = MathHelper.Max(m_EnginePower - deltaTime / GameVariables.PlayerEngineSpoolUpTime, 0f);
        }

        private void Brake(float deltaTime)
        {
            Steer(Vector2.Zero, deltaTime);
        }

        // Eases m_Velocity toward desiredVelocity at a rate capped by
        // PlayerAcceleration -- shared by both thrust (desiredVelocity
        // along the current heading) and braking (desiredVelocity = zero).
        private void Steer(Vector2 desiredVelocity, float deltaTime)
        {
            Vector2 steering = desiredVelocity - m_Velocity;
            float maxAccel = GameVariables.PlayerAcceleration * deltaTime;

            if (steering.Length() > maxAccel)
            {
                steering.Normalize();
                steering *= maxAccel;
            }

            m_Velocity += steering;
        }

        private void UpdateGamePad(GamePadState gamePadState)
        {
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

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                // The ship's art is drawn nose-up (SpriteBatch's rotation=0
                // faces north), but Rotation itself is computed via
                // Vector2.ToAngle()/atan2 (0 = east) to stay consistent with
                // the forward-vector math in UpdateSteering -- reconciled
                // here, the one place both conventions meet, so the
                // rendered heading matches the ship's actual direction of
                // travel instead of sitting 90 degrees off from it.
                spriteBatch.DrawSafe(Texture, Position, null, Color.White, Rotation + MathHelper.PiOver2,
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
