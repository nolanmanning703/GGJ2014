using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

        public static SpriteFont DebugFont;

        public static List<Vector2> GetJousterPolygon(ZoomLevels zoom)
        {
            switch (zoom)
            {
                    //TODO: Fill.
                case ZoomLevels.One:
                    return null;
                case ZoomLevels.Two:
                    return null;
                case ZoomLevels.Three:
                    return null;
                case ZoomLevels.Four:
                    return null;
                case ZoomLevels.Five:
                    return null;
                default: throw new NotImplementedException();
            }
        }


        public static void Initialize(GraphicsDevice gd, ContentManager content)
        {
            WorldBackgrounds[ZoomLevels.One] = content.Load<Texture2D>("Art/Z1");
            WorldBackgrounds[ZoomLevels.Two] = content.Load<Texture2D>("Art/Z2");
            WorldBackgrounds[ZoomLevels.Three] = content.Load<Texture2D>("Art/Z3");
            WorldBackgrounds[ZoomLevels.Four] = content.Load<Texture2D>("Art/Z4");
            WorldBackgrounds[ZoomLevels.Five] = content.Load<Texture2D>("Art/Z5");

            DebugFont = content.Load<SpriteFont>("DebugFont");
        }
    }
}
