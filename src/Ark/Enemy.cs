#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class Enemy : Sprite
    {
        #region Constants

        private const int m_MaxLasers = 6;

        #endregion

        #region Private Members

        private Viewport m_Viewport;

        private Rectangle m_ViewportRect;
        private Rectangle m_BoundingRect;

        private Vector2 m_Center;

        private float m_Health;
        private float m_CurrentHealth;
        private float m_Radius;
        private float m_LaserTime;
        private float m_NextFireInterval;
        private float m_Speed;

        private float m_LateralVelocity;
        private float m_WanderPhase;

        private Laser[] m_Lasers;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        public float CurrentHealth
        {
            get { return m_CurrentHealth; }
            set
            {
                m_CurrentHealth = value;

                if (m_CurrentHealth <= 0)
                {
                    IsAlive = false;
                }
            }
        }

        public Rectangle BoundingRect
        {
            get { return m_BoundingRect; }
            set { m_BoundingRect = value; }
        }

        public Laser[] Lasers
        {
            get { return m_Lasers; }
            set { m_Lasers = value; }
        }

        public bool IsDead
        {
            get { return m_CurrentHealth <= 0; }
        }

        public bool IsBeingPulled { get; set; }

        // A gravity bomb can pull an enemy above the top of the viewport
        // (there's no upper wrap the way there is for the bottom edge in
        // UpdateMovement) -- gate anything that shouldn't happen while
        // invisible, like firing, on this.
        public bool IsOnScreen
        {
            get { return !Physics.IsOutOfBounds(Position, m_ViewportRect); }
        }

        #endregion

        #region Initialisation

        public Enemy(GraphicsDevice graphicsDevice, Vector2 position, float health, float speed)
        {
            m_Viewport = graphicsDevice.Viewport;

            m_ViewportRect = new Rectangle(m_Viewport.X, m_Viewport.Y,
                m_Viewport.Width, m_Viewport.Height);

            Texture = ContentManager.Enemy;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);

                Position = position;

                m_Center = new Vector2(Position.X + Width / 2, 
                    Position.Y + Height / 2);

                BoundingRect = new Rectangle((int)Position.X - (int)Origin.X,
                    (int)Position.Y - (int)Origin.Y, Width, Height);

                m_Health = health;
                m_CurrentHealth = m_Health;
                m_Speed = speed;

                IsAlive = true;
            }

            m_Radius = 1000;

            // Randomized per-enemy so a wave doesn't fire/wander in
            // lockstep -- rolled once here (not just after the first shot)
            // so even the opening shot isn't synchronized across enemies.
            m_NextFireInterval = GameVariables.EnemyLaserFireInterval
                + Extensions.Random.NextFloat(-GameVariables.EnemyFireIntervalJitter, GameVariables.EnemyFireIntervalJitter);
            m_WanderPhase = Extensions.Random.NextFloat(0f, MathHelper.TwoPi);

            m_Lasers = new Laser[m_MaxLasers];

            for (int i = 0; i < m_MaxLasers; i++)
            {
                m_Lasers[i] = new Laser();

                if (i % 2 == 0)
                {
                    m_Lasers[i].Position.X = this.Position.X - (Width / 2);
                    m_Lasers[i].Position.Y = this.Position.Y;
                }
                else
                {
                    m_Lasers[i].Position.X = this.Position.X + (Width / 2);
                    m_Lasers[i].Position.Y = this.Position.Y;
                }
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            m_BoundingRect.X = (int)Position.X - (int)Origin.X;
            m_BoundingRect.Y = (int)Position.Y - (int)Origin.Y;

            UpdateLaser(gameTime);
        }

        // Separate from Update(GameTime) because Sprite's Update signature
        // is fixed and has no way to carry the live player-projectile list
        // this needs for evasion -- same reason Player.UpdateWeapons is
        // separate from Player.Update. Called explicitly by Wave right
        // after Update(gameTime).
        public void UpdateMovement(GameTime gameTime, List<Projectile> threats)
        {
            if (IsBeingPulled)
            {
                return;
            }

            float targetLateralVelocity = ComputeSteeringTarget(threats);

            m_LateralVelocity = MathHelper.Lerp(m_LateralVelocity, targetLateralVelocity, GameVariables.EnemySteeringBlendPerFrame);

            Position.X += m_LateralVelocity;
            Position.X = MathHelper.Clamp(Position.X, m_ViewportRect.Left + Width / 2, m_ViewportRect.Right - Width / 2);

            Position.Y += m_Speed;

            if (Position.Y > m_ViewportRect.Height)
            {
                Position.X = Extensions.Random.Next(GameVariables.EnemySpawnMinX, GameVariables.EnemySpawnMaxX);
                Position.Y = Extensions.Random.Next(GameVariables.EnemySpawnMinY, GameVariables.EnemySpawnMaxY);
            }
        }

        // Reacts to the soonest-arriving Laser/Railgun shot whose predicted
        // closest approach is both close enough and soon enough to count as
        // a real threat; falls back to a gentle ambient wander otherwise.
        private float ComputeSteeringTarget(List<Projectile> threats)
        {
            Projectile nearestThreat = null;
            Vector2 nearestClosestPoint = Vector2.Zero;
            float nearestTime = float.MaxValue;

            foreach (Projectile projectile in threats)
            {
                float t = Physics.TimeToClosestApproach(projectile.Position, projectile.Velocity, Position);

                if (t > GameVariables.EnemyEvasionLookaheadFrames)
                {
                    continue;
                }

                Vector2 closestPoint = projectile.Position + projectile.Velocity * t;

                if (Vector2.Distance(closestPoint, Position) > GameVariables.EnemyEvasionRadius)
                {
                    continue;
                }

                if (t < nearestTime)
                {
                    nearestTime = t;
                    nearestThreat = projectile;
                    nearestClosestPoint = closestPoint;
                }
            }

            if (nearestThreat != null)
            {
                return DodgeVelocity(nearestThreat.Velocity, nearestClosestPoint);
            }

            m_WanderPhase += GameVariables.EnemyWanderFrequency;

            return (float)Math.Sin(m_WanderPhase) * GameVariables.EnemyWanderSpeed;
        }

        // Perpendicular to the shot's velocity, signed toward whichever
        // side this enemy is already offset from its line -- continuing
        // the lean it's already on rather than picking an arbitrary side.
        private float DodgeVelocity(Vector2 threatVelocity, Vector2 closestPoint)
        {
            if (threatVelocity.LengthSquared() < 0.01f)
            {
                return 0f;
            }

            Vector2 perpendicular = new Vector2(-threatVelocity.Y, threatVelocity.X);
            perpendicular.Normalize();

            float side = Vector2.Dot(Position - closestPoint, perpendicular) >= 0 ? 1f : -1f;

            return perpendicular.X * side * GameVariables.EnemyEvasionSpeed;
        }

        private void UpdateLaser(GameTime gameTime)
        {
            m_LaserTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < m_Lasers.Length; i++)
            {
                m_Lasers[i].Update(gameTime);

                if(m_Lasers[i].IsAlive)
                {
                    if (Physics.IsOutOfBounds(m_Lasers[i].Position, m_ViewportRect))
                    {
                        m_Lasers[i].IsAlive = false;
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                foreach (Laser laser in m_Lasers)
                {
                    laser.Draw(spriteBatch);
                }

                spriteBatch.DrawSafe(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);
            }
        }

        #endregion

        #region Helper Methods

        public void FireLaser()
        {
            if (m_LaserTime >= m_NextFireInterval)
            {
                for (int i = 0; i < m_MaxLasers; i++)
                {
                    if (!m_Lasers[i].IsAlive)
                    {
                        m_Lasers[i].IsAlive = true;
                        m_Lasers[i].Velocity = new Vector2(0, 10);

                        if (i % 2 == 1)
                        {
                            m_Lasers[i].Position = new Vector2(this.Position.X + 14, this.Position.Y + 10);
                        }
                        else
                        {
                            m_Lasers[i].Position = new Vector2(this.Position.X - 15, this.Position.Y + 3);
                        }

                        m_LaserTime = 0;
                        m_NextFireInterval = GameVariables.EnemyLaserFireInterval
                            + Extensions.Random.NextFloat(-GameVariables.EnemyFireIntervalJitter, GameVariables.EnemyFireIntervalJitter);

                        return;
                    }
                }
            }
        }

        public bool IsInRange(Vector2 position)
        {
            return Physics.IsWithinRadius(m_Center, position, m_Radius);
        }

        #endregion
    }
}