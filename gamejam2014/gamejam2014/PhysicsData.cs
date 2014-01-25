using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gamejam2014.Jousting;

namespace gamejam2014
{
    /// <summary>
    /// Holds physics constants and such.
    /// </summary>
    public static class PhysicsData
    {
        public static Dictionary<ZoomLevels, JousterPhysicsData> JoustingMinigamePhysics = new Dictionary<ZoomLevels, JousterPhysicsData>()
        {
            { ZoomLevels.One, new JousterPhysicsData(ZoomLevels.One, 1000.0f, 2.75f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Two, new JousterPhysicsData(ZoomLevels.Two, 1000.0f, 2.75f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Three, new JousterPhysicsData(ZoomLevels.Three, 1000.0f, 2.75f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Four, new JousterPhysicsData(ZoomLevels.Four, 1000.0f, 2.75f, 450.0f, 150.0f, 0.85f) },
            { ZoomLevels.Five, new JousterPhysicsData(ZoomLevels.Five, 1000.0f, 2.75f, 450.0f, 150.0f, 0.85f) },
        };

        /// <summary>
        /// Minimum relative speeds necessary to register a collision between players for each minigame.
        /// </summary>
        public static Dictionary<ZoomLevels, float> MinHitSpeeds = new Dictionary<ZoomLevels, float>()
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
    }
}
