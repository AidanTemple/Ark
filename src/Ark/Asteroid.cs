#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

namespace Ark
{
    public enum AsteroidSize
    {
        Small,
        Medium,
        Large
    }

    public class Asteroid : Sprite
    {
        #region Private Members

        private GraphicsDevice m_GraphicsDevice;

        private float m_Health;
        private float m_CurrentHealth;

        private float m_SpeedMin;
        private float m_SpeedMax;

        private float m_RotationSpeed;

        #endregion

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }

        public AsteroidSize Size { get; private set; }

        public float CollisionDamage { get; private set; }

        public Vector2 Velocity { get; set; }

        public bool IsBeingPulled { get; set; }

        public Rectangle BoundingRect { get; private set; }

        #endregion

        #region Initialisation

        // direction only needs to point the right way -- it doesn't need to
        // be pre-normalized or pre-scaled to a speed. This asteroid's tier
        // determines its own speed range (via SetTier below), so callers
        // (spawning or fragmenting) only ever need to supply a heading.
        public Asteroid(GraphicsDevice graphicsDevice, Vector2 position, Vector2 direction, AsteroidSize size)
        {
            m_GraphicsDevice = graphicsDevice;

            Position = position;

            // Small per-instance tumble -- purely cosmetic, direction/speed
            // randomized so a field of asteroids doesn't spin in lockstep.
            m_RotationSpeed = Extensions.Random.NextFloat(-0.02f, 0.02f);

            SetTier(size);

            if (direction == Vector2.Zero)
            {
                direction = Vector2.UnitY;
            }
            else
            {
                direction.Normalize();
            }

            Velocity = direction * Extensions.Random.NextFloat(m_SpeedMin, m_SpeedMax);

            IsAlive = true;
        }

        // Applies the given size's texture/health/collision-damage/speed and
        // resets CurrentHealth to that tier's max -- used both by the
        // constructor and by TakeDamage when an asteroid shrinks a tier
        // rather than dying outright.
        private void SetTier(AsteroidSize size)
        {
            Size = size;

            switch (size)
            {
                case AsteroidSize.Large:
                    Texture = ContentManager.AsteroidLarge;
                    m_Health = GameVariables.AsteroidHealthLarge;
                    CollisionDamage = GameVariables.AsteroidCollisionDamageLarge;
                    m_SpeedMin = GameVariables.AsteroidSpeedMinLarge;
                    m_SpeedMax = GameVariables.AsteroidSpeedMaxLarge;
                    break;

                case AsteroidSize.Medium:
                    Texture = ContentManager.AsteroidMedium;
                    m_Health = GameVariables.AsteroidHealthMedium;
                    CollisionDamage = GameVariables.AsteroidCollisionDamageMedium;
                    m_SpeedMin = GameVariables.AsteroidSpeedMinMedium;
                    m_SpeedMax = GameVariables.AsteroidSpeedMaxMedium;
                    break;

                default:
                    Texture = ContentManager.AsteroidSmall;
                    m_Health = GameVariables.AsteroidHealthSmall;
                    CollisionDamage = GameVariables.AsteroidCollisionDamageSmall;
                    m_SpeedMin = GameVariables.AsteroidSpeedMinSmall;
                    m_SpeedMax = GameVariables.AsteroidSpeedMaxSmall;
                    break;
            }

            m_CurrentHealth = m_Health;

            if (Texture != null)
            {
                Width = Texture.Width;
                Height = Texture.Height;

                Origin = new Vector2(Width / 2, Height / 2);
            }

            RecomputeBoundingRect();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime)
        {
            if (!IsBeingPulled)
            {
                Position += Velocity;
            }

            Rotation += m_RotationSpeed;

            RecomputeBoundingRect();
        }

        private void RecomputeBoundingRect()
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
                spriteBatch.Draw(Texture, Position, null, Color.White, Rotation,
                    Origin, Scale, SpriteEffects.None, Depth);
            }
        }

        #endregion

        #region Helper Methods

        // Health hitting zero means something different per tier -- for
        // Small it's death (nothing smaller exists), for Large/Medium it
        // means "shrink a tier", which is why this isn't a CurrentHealth
        // setter side effect the way Enemy.CurrentHealth is: reaching zero
        // doesn't always mean IsAlive should become false.
        //
        // Returns the newly-created fragment Asteroid if this hit caused a
        // tier-down, or null if it didn't (not lethal this hit, or this was
        // the terminal Small-tier kill).
        public Asteroid TakeDamage(float damage, Vector2 impactDirection)
        {
            m_CurrentHealth -= damage;

            if (m_CurrentHealth > 0)
            {
                return null;
            }

            if (Size == AsteroidSize.Small)
            {
                IsAlive = false;
                return null;
            }

            AsteroidSize fragmentSize = Size == AsteroidSize.Large ? AsteroidSize.Medium : AsteroidSize.Small;

            // This asteroid shrinks to the new tier in place, keeping its
            // existing Position/Velocity, rather than dying -- "reduce the
            // size of an asteroid" -- while a second, independent Asteroid
            // at the same new tier is flung off from the impact -- "a
            // smaller asteroid chunk to move off". Its own speed for the new
            // tier is picked by its constructor, same as any other spawn.
            SetTier(fragmentSize);

            return new Asteroid(m_GraphicsDevice, Position, Deflect(impactDirection), fragmentSize);
        }

        // Rotates direction by a random angle within
        // +/-AsteroidFragmentAngleSpreadDegrees, so a fragment scatters away
        // from the impact instead of continuing straight through it.
        private static Vector2 Deflect(Vector2 direction)
        {
            if (direction == Vector2.Zero)
            {
                direction = Vector2.UnitY;
            }
            else
            {
                direction.Normalize();
            }

            float spread = MathHelper.ToRadians(GameVariables.AsteroidFragmentAngleSpreadDegrees);
            float angle = Extensions.Random.NextFloat(-spread, spread);

            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            return new Vector2(direction.X * cos - direction.Y * sin, direction.X * sin + direction.Y * cos);
        }

        #endregion
    }
}
