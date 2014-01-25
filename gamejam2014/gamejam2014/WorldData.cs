using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using gamejam2014.Jousting;

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
            { ZoomLevels.Two, 0.1f },
            { ZoomLevels.Three, 1.0f },
            { ZoomLevels.Four, 10.0f },
            { ZoomLevels.Five, 100.0f },
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

        public static ZoomLevels ZoomOut(ZoomLevels current)
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
        public static ZoomLevels ZoomIn(ZoomLevels current)
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

        public struct IntermediateZoom
        {
            public ZoomLevels Inner, Outer;
            /// <summary>
            /// 0 = Inner, 1 = Outer
            /// </summary>
            public float LinearInterpolant;
            public override string ToString()
            {
                return "[" + Inner.ToString() + "," + Outer.ToString() + "] : " + LinearInterpolant.ToString();
            }
        }
        public static IntermediateZoom GetCurrentZoom(float zoomLevel)
        {
            foreach (ZoomLevels zoom in AscendingZooms)
            {
                ZoomLevels zoomOut = ZoomOut(zoom);
                if (ZoomScaleAmount[zoom] <= zoomLevel &&
                    ZoomScaleAmount[zoomOut] > zoomLevel)
                {
                    IntermediateZoom ret = new IntermediateZoom();
                    ret.Inner = zoom;
                    ret.Outer = zoomOut;
                    //Use logarithmic interpolant.
                    float lerpComponent = (zoomLevel - ZoomScaleAmount[zoom]) / (ZoomScaleAmount[zoomOut] - ZoomScaleAmount[zoom]);
                    if (lerpComponent == 0.0f) ret.LinearInterpolant = 0.0f;
                    else ret.LinearInterpolant = -(float)Math.Log(lerpComponent, 10.0f);
                    return ret;
                }
            }

            return new IntermediateZoom();
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
