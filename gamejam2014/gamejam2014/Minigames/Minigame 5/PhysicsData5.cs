using System;
using System.Collections.Generic;
using Utilities.Math;
using Utilities.Math.Shape;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_5
{
    public static class PhysicsData5
    {
        private static Circle BlackHole = new Circle(V2.Zero, 100.0f);
        public static Circle GetBlackHole()
        {
            float scale = WorldData.ZoomScaleAmount[KarmaWorld.World.CurrentZoom];
            return new Circle(scale * BlackHole.Center, scale * BlackHole.Radius);
        }

        public static float BlackHolePushBack = 3000.0f;
        public static float BlackHoleDamage = 0.1f;

        private static Interval BlackHolePullAccel = new Interval(50.0f, 500.0f, true, 5);
        private static float BlackHoleDropoff = 2.0f;
        public static float GetBlackHolePull(float distance, float zoomScale)
        {
            float minDist = GetBlackHole().Radius;
            float maxDist = KarmaWorld.World.WorldSize.Length();
            maxDist -= minDist;

            float interpolant = ((distance / zoomScale) - minDist) / (maxDist - minDist);
            interpolant = Microsoft.Xna.Framework.MathHelper.Clamp(interpolant, 1.0f, 0.0f);
            interpolant = (float)System.Math.Pow(interpolant, BlackHoleDropoff);

            return zoomScale * Microsoft.Xna.Framework.MathHelper.Lerp(BlackHolePullAccel.Start, BlackHolePullAccel.End, interpolant);
        }
    }
}
