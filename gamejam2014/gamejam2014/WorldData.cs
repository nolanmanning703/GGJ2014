using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using gamejam2014.Jousting;
using gamejam2014.Minigames;

namespace gamejam2014
{
    public enum ZoomLevels
    {
        One = 1,
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
        public static float CameraZoomSpeedScale = 35.0f;

        public static int ZoomToInt(ZoomLevels zoom)
        {
            return (int)zoom;
        }

        public static IEnumerable<ZoomLevels> AscendingZooms
        {
            get
            {
                ZoomLevels z = ZoomLevels.One;
                while (z != ZoomLevels.Five)
                {
                    yield return z;
                    z = ZoomOut(z);
                }
                yield return ZoomLevels.Five;
            }
        }
        public static IEnumerable<ZoomLevels> DescendingZooms
        {
            get
            {
                ZoomLevels z = ZoomLevels.Five;
                while (z != ZoomLevels.One)
                {
                    yield return z;
                    z = ZoomIn(z);
                }
                yield return ZoomLevels.One;
            }
        }

        public static ZoomLevels ZoomOut(ZoomLevels current)
        {
            switch (current)
            {
                case ZoomLevels.One: return ZoomLevels.Three;
                case ZoomLevels.Two: return ZoomLevels.Three;
                case ZoomLevels.Three: return ZoomLevels.Five;
                case ZoomLevels.Four: return ZoomLevels.Five;
                case ZoomLevels.Five: return ZoomLevels.Five;
                default: throw new NotImplementedException();
            }

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
                case ZoomLevels.Five: return ZoomLevels.Three;
                case ZoomLevels.Four: return ZoomLevels.Three;
                case ZoomLevels.Three: return ZoomLevels.One;
                case ZoomLevels.Two: return ZoomLevels.One;
                case ZoomLevels.One: return ZoomLevels.One;
                default: throw new NotImplementedException();
            }

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
        public static ZoomLevels ZoomInverse(ZoomLevels current)
        {
            switch (current)
            {
                case ZoomLevels.One: return ZoomLevels.Five;
                case ZoomLevels.Two: return ZoomLevels.Four;
                case ZoomLevels.Three: return ZoomLevels.Three;
                case ZoomLevels.Four: return ZoomLevels.Two;
                case ZoomLevels.Five: return ZoomLevels.One;
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
            { ZoomLevels.One, new Minigames.Minigame_1.Minigame1(ZoomLevels.One) },
            { ZoomLevels.Two, null },
            { ZoomLevels.Three, new TestMinigame(ZoomLevels.Three) },
            { ZoomLevels.Four, null },
            { ZoomLevels.Five, new Minigames.Minigame_5.Minigame5(ZoomLevels.Five) },
        };


        private static Dictionary<ZoomLevels, Dictionary<Jousters, Vector2>> StartingPoses = new Dictionary<ZoomLevels, Dictionary<Jousters, Vector2>>()
        {
            { ZoomLevels.One,
              new Dictionary<Jousters, Vector2>()
              {
                  { Jousters.Harmony, new Vector2(500.0f, 500.0f) },
                  { Jousters.Dischord, new Vector2(1000.0f, 1000.0f) },
              }
            },
            { ZoomLevels.Two,
              new Dictionary<Jousters, Vector2>()
              {
                  { Jousters.Harmony, new Vector2(500.0f, 500.0f) },
                  { Jousters.Dischord, new Vector2(1000.0f, 1000.0f) },
              }
            },
            { ZoomLevels.Three,
              new Dictionary<Jousters, Vector2>()
              {
                  { Jousters.Harmony, new Vector2(500.0f, 500.0f) },
                  { Jousters.Dischord, new Vector2(1000.0f, 1000.0f) },
              }
            },
            { ZoomLevels.Four,
              new Dictionary<Jousters, Vector2>()
              {
                  { Jousters.Harmony, new Vector2(500.0f, 500.0f) },
                  { Jousters.Dischord, new Vector2(1000.0f, 1000.0f) },
              }
            },
            { ZoomLevels.Five,
              new Dictionary<Jousters, Vector2>()
              {
                  { Jousters.Harmony, new Vector2(500.0f, 500.0f) },
                  { Jousters.Dischord, new Vector2(1000.0f, 1000.0f) },
              }
            },
        };
        public static Vector2 GetStartingPos(ZoomLevels zoom, Jousters jouster)
        {
            return ZoomScaleAmount[zoom] * StartingPoses[zoom][jouster];
        }

        public static Utilities.Math.Interval SpecialGainPerSecond = new Utilities.Math.Interval(0.05f, 0.15f, true, true, true, 4);

        public static void Initialize(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
        {

        }
    }
}
