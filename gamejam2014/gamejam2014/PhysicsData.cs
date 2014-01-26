using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gamejam2014.Jousting;
using Utilities.Math;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014
{
    /// <summary>
    /// Holds physics constants and such.
    /// </summary>
    public static class PhysicsData
    {
        public static Dictionary<ZoomLevels, JousterPhysicsData> JoustingMinigamePhysics = new Dictionary<ZoomLevels, JousterPhysicsData>()
        {
            { ZoomLevels.One, new JousterPhysicsData(ZoomLevels.One, 1000.0f, 4.0f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Two, new JousterPhysicsData(ZoomLevels.Two, 1000.0f, 4.0f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Three, new JousterPhysicsData(ZoomLevels.Three, 1000.0f, 4.0f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Four, new JousterPhysicsData(ZoomLevels.Four, 1000.0f, 4.0f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Five, new JousterPhysicsData(ZoomLevels.Five, 1000.0f, 4.0f, 600.0f, 350.0f, 0.85f) },
        };

        public static Dictionary<ZoomLevels, float> JousterStartingMass = new Dictionary<ZoomLevels, float>()
        {
            { ZoomLevels.One, 1.0f },
            { ZoomLevels.Two, 1.0f },
            { ZoomLevels.Three, 1.0f },
            { ZoomLevels.Four, 1.0f },
            { ZoomLevels.Five, 1.0f },
        };

        /// <summary>
        /// Minimum relative speeds necessary to register a collision between players for each minigame.
        /// </summary>
        private static Dictionary<ZoomLevels, float> MinHitSpeeds = new Dictionary<ZoomLevels, float>()
        {
            { ZoomLevels.One, 100.0f },
            { ZoomLevels.Two, 100.0f },
            { ZoomLevels.Three, 100.0f },
            { ZoomLevels.Four, 100.0f },
            { ZoomLevels.Five, 100.0f },
        };
        /// <summary>
        /// Gets actual minimum hit speed for the given zoom level, accounting for level zoom scale.
        /// </summary>
        public static float GetMinHitSpeed(ZoomLevels zoom)
        {
            return WorldData.ZoomScaleAmount[zoom] *
                   MinHitSpeeds[zoom];
        }


        //Reacting to hits from other players.

        public static Vector2 VelocityFromHit(float stabDamage, Vector2 hitDir)
        {
            return hitDir * stabDamage;
        }
        public static float VelocityDampFromHit(float stabDamage, float maxSpeed)
        {
            return new Interval(-0.5f, maxSpeed, true, 2).Map(Interval.ZeroToOneInterval, stabDamage);
        }

        private static Interval RelativeHitDamageRange = new Interval(0.0f, 1000.0f, true, 1);
        private static float MaxDamageFromHit = 0.5f;
        public static float GetDamage(float stabStrength, float zoomScale)
        {
            return MaxDamageFromHit * RelativeHitDamageRange.Clamp(RelativeHitDamageRange.Map(Interval.ZeroToOneInterval, stabStrength / zoomScale));
        }
    }
}
