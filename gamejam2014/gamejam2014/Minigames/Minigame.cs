using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gamejam2014
{
    /// <summary>
    /// A single zoom level game.
    /// </summary>
    public abstract class Minigame
    {
        protected KarmaWorld World { get { return KarmaWorld.World; } }
        protected Microsoft.Xna.Framework.GameTime Time { get { return KarmaWorld.World.CurrentTime; } }

        /// <summary>
        /// If true, this game has ended and should move upwards to the minigame above this one.
        /// </summary>
        public bool MoveUp { get; set; }
        /// <summary>
        /// If true, this game has ended and should move downwards to the minigame below this one.
        /// </summary>
        public bool MoveDown { get; set; }

        public Minigame() { MoveUp = false; MoveDown = false; }

        public void ResetGame() { MoveUp = false; MoveDown = false; Reset(); }

        protected abstract void Reset();
        public abstract void Update();
        public abstract void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb);
    }
}
