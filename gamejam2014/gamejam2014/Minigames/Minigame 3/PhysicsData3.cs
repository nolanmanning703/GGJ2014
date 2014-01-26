using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Math;
using Utilities.Math.Shape;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_3
{
    public static class PhysicsData3
    {
        private static V2 HillCenter = new V2(0.0f, -90.0f);
        public static Circle GetHillCircle(float zoomScale) { return new Circle(HillCenter * zoomScale, ArtAssets3.HillRadius * zoomScale); }

        public static float TimeInHillToWin = 60.0f;

        private static V2 DoghouseCenter = new V2(0.0f, -200.0f);
        public static V2 GetDoghouseCenter(float zoomScale) { return zoomScale * DoghouseCenter; }

        public static float SpecialStunTime = 6.5f;

        public static int NumbTennisBalls = 10;
        public static float TennisBallMass = 0.01f;
        public static float TennisBallMaxVelocity = 850.0f;
    }
}
