using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Utilities.Graphics;

namespace gamejam2014
{
    /// <summary>
    /// All the art assets.
    /// </summary>
    public static class ArtAssets
    {
        public static Dictionary<ZoomLevels, Texture2D> WorldBackgrounds = new Dictionary<ZoomLevels, Texture2D>()
        {
            { ZoomLevels.One, null },
            { ZoomLevels.Two, null },
            { ZoomLevels.Three, null },
            { ZoomLevels.Four, null },
            { ZoomLevels.Five, null },
        };
        public static Vector2 GetBackgroundOrigin(ZoomLevels zoom)
        {
            return 0.5f * new Vector2(WorldBackgrounds[zoom].Width, WorldBackgrounds[zoom].Height);
        }
        public static Dictionary<ZoomLevels, float> WorldBackgroundLayerMaxes = new Dictionary<ZoomLevels, float>()
        {
            { ZoomLevels.Five, 1.0f },
            { ZoomLevels.Four, 0.75f },
            { ZoomLevels.Three, 0.5f },
            { ZoomLevels.Two, 0.25f },
            { ZoomLevels.One, 0.0f },
        };

        public static Dictionary<ZoomLevels, Dictionary<Jousting.Jousters, AnimatedSprite>> PlayerSprites = new Dictionary<ZoomLevels,Dictionary<Jousting.Jousters,AnimatedSprite>>()
        {
            { ZoomLevels.One,
                new Dictionary<Jousting.Jousters, AnimatedSprite>()
                {
                    { Jousting.Jousters.Dischord, null },
                    { Jousting.Jousters.Harmony, null },
                }
            },
            { ZoomLevels.Two,
                new Dictionary<Jousting.Jousters, AnimatedSprite>()
                {
                    { Jousting.Jousters.Dischord, null },
                    { Jousting.Jousters.Harmony, null },
                }
            },
            { ZoomLevels.Three,
                new Dictionary<Jousting.Jousters, AnimatedSprite>()
                {
                    { Jousting.Jousters.Dischord, null },
                    { Jousting.Jousters.Harmony, null },
                }
            },
            { ZoomLevels.Four,
                new Dictionary<Jousting.Jousters, AnimatedSprite>()
                {
                    { Jousting.Jousters.Dischord, null },
                    { Jousting.Jousters.Harmony, null },
                }
            },
            { ZoomLevels.Five,
                new Dictionary<Jousting.Jousters, AnimatedSprite>()
                {
                    { Jousting.Jousters.Dischord, null },
                    { Jousting.Jousters.Harmony, null },
                }
            },
        };

        public static SpriteFont DebugFont;

        public static List<Vector2> GetJousterPolygon(ZoomLevels zoom)
        {
            float scale = WorldData.ZoomScaleAmount[zoom];
            switch (zoom)
            {
                case ZoomLevels.One:
                    return new List<Vector2>()
                    {
                        new Vector2(64.0f, 32.0f) * scale,
                        new Vector2(12.0f, 63.0f) * scale,
                        new Vector2(0.0f, 64.0f) * scale,
                        new Vector2(0.0f, 0.0f) * scale,
                        new Vector2(12.0f, 1.0f) * scale,
                    };
                case ZoomLevels.Two:
                    return new List<Vector2>()
                    {
                        new Vector2(64.0f, 32.0f) * scale,
                        new Vector2(12.0f, 63.0f) * scale,
                        new Vector2(0.0f, 64.0f) * scale,
                        new Vector2(0.0f, 0.0f) * scale,
                        new Vector2(12.0f, 1.0f) * scale,
                    };
                case ZoomLevels.Three:
                    return new List<Vector2>()
                    {
                        new Vector2(64.0f, 32.0f) * scale,
                        new Vector2(12.0f, 63.0f) * scale,
                        new Vector2(0.0f, 64.0f) * scale,
                        new Vector2(0.0f, 0.0f) * scale,
                        new Vector2(12.0f, 1.0f) * scale,
                    };
                case ZoomLevels.Four:
                    return new List<Vector2>()
                    {
                        new Vector2(64.0f, 32.0f) * scale,
                        new Vector2(12.0f, 63.0f) * scale,
                        new Vector2(0.0f, 64.0f) * scale,
                        new Vector2(0.0f, 0.0f) * scale,
                        new Vector2(12.0f, 1.0f) * scale,
                    };
                case ZoomLevels.Five:
                    return new List<Vector2>()
                    {
                        new Vector2(64.0f, 32.0f) * scale,
                        new Vector2(12.0f, 63.0f) * scale,
                        new Vector2(0.0f, 64.0f) * scale,
                        new Vector2(0.0f, 0.0f) * scale,
                        new Vector2(12.0f, 1.0f) * scale,
                    };
                default: throw new NotImplementedException();
            }
        }


        public static void Initialize(GraphicsDevice gd, ContentManager content)
        {
            foreach (ZoomLevels zoom in WorldData.AscendingZooms)
            {
                string zoomS = WorldData.ZoomToInt(zoom).ToString();

                WorldBackgrounds[zoom] = content.Load<Texture2D>("Art/Z" + zoomS);

                foreach (Jousting.Jousters jouster in Utilities.OtherFunctions.GetValues<Jousting.Jousters>())
                {
                    string jousterS = (jouster == Jousting.Jousters.Dischord ? "C" : "H");
                    PlayerSprites[zoom][jouster] = new AnimatedSprite(content.Load<Texture2D>("Art/Player" + jousterS + " " + zoomS));
                    PlayerSprites[zoom][jouster].SetOriginToCenter();
                }
            }

            DebugFont = content.Load<SpriteFont>("DebugFont");
        }
    }
}
