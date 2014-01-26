using System;
using System.Collections.Generic;
using Utilities.Math;
using Utilities.Math.Shape;

namespace gamejam2014.Minigames.Minigame_1
{
    public static class PhysicsData1
    {
        public static float GoodBacteriaMass = 0.2f,
                            InfectedBacteriaMass = 0.1f;
        public static float BacteriaMassGiveToPlayerScale = 1.0f;

        public static float BacteriaInfectEnergyDamp = 0.1f;

        public static float BacteriaAppearanceTime = 5.0f;

        public static float BacteriaDriftMaxSpeed = 100.0f;
    }
}