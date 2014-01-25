﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace gamejam2014
{
    public enum ZoomLevels
    {
        One,
        Two,
        Three,
        Four,
        Five,
    }

    /// <summary>
    /// Constants and data about the world.
    /// </summary>
    public static class WorldData
    {
        public static float CameraZoomSpeed(ZoomLevels zoom)
        {
            return ZoomScaleAmount[zoom] * 10.0f;
        }
        public static Dictionary<ZoomLevels, float> ZoomScaleAmount = new Dictionary<ZoomLevels, float>()
        {
            { ZoomLevels.One, 0.01f },
            { ZoomLevels.Two, 1.0f },
            { ZoomLevels.Three, 10.0f },
            { ZoomLevels.Four, 100.0f },
            { ZoomLevels.Five, 1000.0f },
        };

        public static IEnumerable<ZoomLevels> AscendingZooms
        {
            get
            {
                yield return ZoomLevels.One;
                yield return ZoomLevels.Two;
                yield return ZoomLevels.Three;
                yield return ZoomLevels.Four;
                yield return ZoomLevels.Five;
            }
        }
        public static IEnumerable<ZoomLevels> DescendingZooms
        {
            get
            {
                yield return ZoomLevels.Five;
                yield return ZoomLevels.Four;
                yield return ZoomLevels.Three;
                yield return ZoomLevels.Two;
                yield return ZoomLevels.One;
            }
        }

        public static ZoomLevels ZoomIn(ZoomLevels current)
        {
            switch (current)
            {
                case ZoomLevels.One: return ZoomLevels.Two;
                case ZoomLevels.Two: return ZoomLevels.Three;
                case ZoomLevels.Three: return ZoomLevels.Four;
                case ZoomLevels.Four: return ZoomLevels.Five;
                case ZoomLevels.Five: return ZoomLevels.Five;
                default: throw new NotImplementedException();
            }
        }
        public static ZoomLevels ZoomOut(ZoomLevels current)
        {
            switch (current)
            {
                case ZoomLevels.Five: return ZoomLevels.Four;
                case ZoomLevels.Four: return ZoomLevels.Three;
                case ZoomLevels.Three: return ZoomLevels.Two;
                case ZoomLevels.Two: return ZoomLevels.One;
                case ZoomLevels.One: return ZoomLevels.One;
                default: throw new NotImplementedException();
            }
        }


        public static Dictionary<ZoomLevels, Minigame> Minigames = new Dictionary<ZoomLevels, Minigame>()
        {
            { ZoomLevels.One, null },
            { ZoomLevels.Two, null },
            { ZoomLevels.Three, null },
            { ZoomLevels.Four, null },
            { ZoomLevels.Five, null },
        };


        public static void Initialize(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
        {

        }
    }
}