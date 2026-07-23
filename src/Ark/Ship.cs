#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

namespace Ark
{
    // Common base for anything that flies around under its own power and
    // carries weapons -- point-to-move steering, weapon pooling/update/draw.
    // Player is the only concrete Ship today (input-driven), but the split
    // exists so a future AI-driven ship type can reuse this exact movement
    // and weapon plumbing instead of duplicating it, the way the old
    // (deleted) Enemy class used to.
    public abstract class Ship : Sprite
    {
        #region Private Members

        private Vector2 m_Destination;
        private Vector2 m_Velocity;

        // 0 (idle) to 1 (full thrust) -- ramps gradually rather than
        // snapping, see UpdateSteering.
        private float m_EnginePower;

        private Rectangle m_BoundingRect;

        // One of each fitted per slot -- null means that slot is empty.
        // See the Create*Module factory methods below.
        private ShieldModule m_ShieldModule;
        private ArmorModule m_ArmorModule;
        private CapacitorModule m_CapacitorModule;
        private PropulsionModule m_PropulsionModule;

        #endregion

        #region Protected Members

        // Weapons this ship carries -- populated by the subclass
        // constructor (Player wraps each in a WeaponSlot for its own
        // gamepad-button/keyboard mapping, but Ship only needs the plain
        // Weapon to update/draw it every frame).
        protected List<Weapon> m_Weapons = new List<Weapon>();

        protected Rectangle m_ViewportRect;

        #endregion

        #region Properties

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Rectangle BoundingRect
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        public float Health { get; protected set; }

        public float ShieldPercent
        {
            get { return m_ShieldModule != null ? m_ShieldModule.Percent : 0f; }
        }

        public float CapacitorPercent
        {
            get { return m_CapacitorModule != null ? m_CapacitorModule.Percent : 0f; }
        }

        protected virtual float MaxHealth => GameVariables.ShipHealth;

        // Movement stats a subclass can override to make a distinct ship
        // type feel different (faster/slower, tighter/wider turns, etc.)
        // without touching UpdateSteering itself. Default to the shared
        // GameVariables.Ship* tuning, with Speed/TurnRateDegrees further
        // boosted by a fitted PropulsionModule, if any.
        protected virtual float Speed =>
            GameVariables.ShipSpeed * (m_PropulsionModule != null ? m_PropulsionModule.SpeedMultiplier : 1f);
        protected virtual float Acceleration => GameVariables.ShipAcceleration;
        protected virtual float TurnRateDegrees =>
            GameVariables.ShipTurnRateDegrees * (m_PropulsionModule != null ? m_PropulsionModule.TurnRateMultiplier : 1f);
        protected virtual float ArrivalRadius => GameVariables.ShipArrivalRadius;
        protected virtual float SlowRadius => GameVariables.ShipSlowRadius;
        protected virtual float BrakingSpeedThreshold => GameVariables.ShipBrakingSpeedThreshold;
        protected virtual float HeadingToleranceDegrees => GameVariables.ShipHeadingToleranceDegrees;
        protected virtual float EngineSpoolUpTime => GameVariables.ShipEngineSpoolUpTime;

        #endregion

        #region Initialisation

        protected Ship(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            Viewport viewport = graphicsDevice.Viewport;

            m_ViewportRect = new Rectangle(viewport.X, viewport.Y,
                viewport.Width, viewport.Height);

            Texture = texture;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                PutInStartPosition();

                BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                    (int)Position.Y - (int)Origin.Y, Width, Height);

                // Start already "arrived" -- otherwise the ship would
                // immediately steer toward Vector2.Zero (top-left).
                m_Destination = Position;

                Health = MaxHealth;

                // One slot of each -- a subclass wanting a different
                // loadout (or none at all) overrides the relevant factory
                // below; returning null leaves that slot empty.
                m_ShieldModule = CreateShieldModule();
                m_ArmorModule = CreateArmorModule();
                m_CapacitorModule = CreateCapacitorModule();
                m_PropulsionModule = CreatePropulsionModule();

                IsAlive = true;
            }
        }

        // Default spawn point -- bottom-center of the viewport, matching
        // where the player ship has always started. Override for a
        // different spawn rule (e.g. a future ship type entering from an
        // edge).
        protected virtual void PutInStartPosition()
        {
            Position = new Vector2(m_ViewportRect.Width / 2, m_ViewportRect.Height - Height);
        }

        // Empty (unfitted) by default -- override to fit a module of that
        // category. A ship type with no shield slot at all, for example,
        // simply doesn't override CreateShieldModule.
        protected virtual ShieldModule CreateShieldModule() => null;
        protected virtual ArmorModule CreateArmorModule() => null;
        protected virtual CapacitorModule CreateCapacitorModule() => null;
        protected virtual PropulsionModule CreatePropulsionModule() => null;

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            UpdateSteering(gameTime);

            Position = Physics.ClampToBounds(Position, m_ViewportRect, Width / 2, Height / 2);

            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            foreach (Weapon weapon in m_Weapons)
            {
                weapon.Update(gameTime, m_ViewportRect);
            }

            m_ShieldModule?.Update(gameTime);
            m_ArmorModule?.Update(gameTime);
            m_CapacitorModule?.Update(gameTime);
            m_PropulsionModule?.Update(gameTime);
        }

        // The single entry point for anything hurting this ship -- armor
        // reduces first, then the shield soaks what's left, and only
        // whatever gets past both comes off Health. Callers must not
        // subtract Health directly, or armor/shield (and the shield's
        // repair-delay timer) are silently bypassed. Nothing calls this
        // yet (no source of incoming damage exists right now), but the
        // pipeline is here and correct for when one does.
        public void TakeDamage(float damage)
        {
            if (m_ArmorModule != null)
            {
                damage = m_ArmorModule.Reduce(damage);
            }

            if (m_ShieldModule != null)
            {
                damage = m_ShieldModule.Absorb(damage);
            }

            if (damage > 0)
            {
                Health -= damage;
            }
        }

        // Locks in a new point-to-move target -- called by whatever drives
        // this ship (Player's mouse/gamepad input today, an AI controller
        // for a future ship type).
        protected void SetDestination(Vector2 destination)
        {
            m_Destination = destination;
        }

        // Ship-like steering toward m_Destination, with rotation and thrust
        // mutually exclusive -- the ship never turns while it still has
        // meaningful velocity, and never thrusts until it's pointed the
        // right way (within HeadingToleranceDegrees). Redirecting mid-flight
        // to a new destination therefore plays out in three phases every
        // real vessel goes through: brake off the old heading's velocity,
        // rotate in place once nearly stopped, then accelerate along the
        // new heading. This is a standard pattern for non-arcade ship
        // autopilots (e.g. EVE Online's align-then-approach behavior) --
        // not a strafing Asteroids-style ship that can thrust in any
        // direction regardless of facing.
        private void UpdateSteering(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 toDestination = m_Destination - Position;
            float distance = toDestination.Length();

            if (distance <= ArrivalRadius)
            {
                // Arrived -- nothing left to point at, just coast to a stop.
                SpoolDown(deltaTime);
            }
            else
            {
                float desiredHeading = toDestination.ToAngle();
                float wrappedDiff = MathHelper.WrapAngle(desiredHeading - Rotation);
                float toleranceRadians = MathHelper.ToRadians(HeadingToleranceDegrees);

                if (Math.Abs(wrappedDiff) > toleranceRadians)
                {
                    // Not pointed the right way. Only start turning once
                    // any existing velocity has bled off below the braking
                    // threshold -- otherwise keep braking instead.
                    bool isStopped = m_Velocity.LengthSquared() <=
                        BrakingSpeedThreshold * BrakingSpeedThreshold;

                    if (isStopped)
                    {
                        float maxTurn = MathHelper.ToRadians(TurnRateDegrees) * deltaTime;
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
                    m_EnginePower = MathHelper.Min(m_EnginePower + deltaTime / EngineSpoolUpTime, 1f);

                    float speedFactor = MathHelper.Clamp(distance / SlowRadius, 0f, 1f);
                    Vector2 forward = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));

                    Steer(forward * Speed * speedFactor * m_EnginePower, deltaTime);
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
            m_EnginePower = MathHelper.Max(m_EnginePower - deltaTime / EngineSpoolUpTime, 0f);
        }

        private void Brake(float deltaTime)
        {
            Steer(Vector2.Zero, deltaTime);
        }

        // Eases m_Velocity toward desiredVelocity at a rate capped by
        // Acceleration -- shared by both thrust (desiredVelocity along the
        // current heading) and braking (desiredVelocity = zero).
        private void Steer(Vector2 desiredVelocity, float deltaTime)
        {
            Vector2 steering = desiredVelocity - m_Velocity;
            float maxAccel = Acceleration * deltaTime;

            if (steering.Length() > maxAccel)
            {
                steering.Normalize();
                steering *= maxAccel;
            }

            m_Velocity += steering;
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                // Ship art is drawn nose-up (SpriteBatch's rotation=0 faces
                // north), but Rotation itself is computed via
                // Vector2.ToAngle()/atan2 (0 = east) to stay consistent with
                // the forward-vector math in UpdateSteering -- reconciled
                // here, the one place both conventions meet, so the
                // rendered heading matches the ship's actual direction of
                // travel instead of sitting 90 degrees off from it.
                spriteBatch.DrawSafe(Texture, Position, null, Color.White, Rotation + MathHelper.PiOver2,
                    Origin, Scale, SpriteEffects.None, Depth);

                foreach (Weapon weapon in m_Weapons)
                {
                    weapon.Draw(spriteBatch);
                }
            }
        }

        #endregion
    }
}
