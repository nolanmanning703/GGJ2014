using System;
using System.Collections.Generic;
using Utilities.Math;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_4
{
    public static class PhysicsData4
    {
        private static Utilities.Math.Shape.Circle Mouth = new Utilities.Math.Shape.Circle(V2.Zero, 25);
        public static Utilities.Math.Shape.Circle GetMouth(ZoomLevels zoom)
        {
            //KarmaWorld.World.CurrentZoom;
            float scale = WorldData.ZoomScaleAmount[zoom];

            return new Utilities.Math.Shape.Circle(scale * Mouth.Center, scale * Mouth.Radius);
        }
    }
}
