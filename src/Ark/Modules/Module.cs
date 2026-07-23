#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Ark
{
    // Common base for ship equipment fitted into one of Ship's module slots
    // (see Ship's Create*Module factory methods). Shields/capacitors
    // regenerate over time and need Update(); armor/propulsion are static
    // stat modifiers and don't, hence the no-op default.
    public abstract class Module
    {
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
