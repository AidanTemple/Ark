#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Ark
{
    public class RailgunProjectile : Projectile
    {
        #region Properties

        public HashSet<Enemy> HitEnemies { get; private set; }
        public HashSet<Asteroid> HitAsteroids { get; private set; }

        #endregion

        #region Initialisation

        public RailgunProjectile(Texture2D texture)
            : base(texture)
        {
            HitEnemies = new HashSet<Enemy>();
            HitAsteroids = new HashSet<Asteroid>();
        }

        #endregion

        #region Update

        public override void Activate(Vector2 position, Vector2 velocity)
        {
            base.Activate(position, velocity);

            HitEnemies.Clear();
            HitAsteroids.Clear();
        }

        #endregion
    }
}
