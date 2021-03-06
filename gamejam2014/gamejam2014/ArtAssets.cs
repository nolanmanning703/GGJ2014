﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Utilities.Graphics;
using Utilities.Math.Shape;
using Rect = Microsoft.Xna.Framework.Rectangle;
using SRect = Utilities.Math.Shape.Rectangle;

namespace gamejam2014
{
    /// <summary>
    /// All the art assets.
    /// </summary>
    public static class ArtAssets
    {
        public static AnimatedSprite EmptySprite;


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

        public static Dictionary<Jousting.Jousters, AnimatedSprite> SpecialUIAlert = new Dictionary<Jousting.Jousters, AnimatedSprite>()
        {
            { Jousting.Jousters.Harmony, null },
            { Jousting.Jousters.Dischord, null },
        };

        public static SpriteFont DebugFont, WorldFont;

        public static Shape GetJousterShape(ZoomLevels zoom, float scale)
        {
            switch (zoom)
            {
                case ZoomLevels.One:
                    return new Polygon(CullMode.CullClockwiseFace, new List<Vector2>()
                                       {
                                           new Vector2(61.0f, 28.0f),
                                           new Vector2(41.0f, 47.0f),
                                           new Vector2(14.0f, 42.0f),
                                           new Vector2(3.0f, 31.0f),
                                           new Vector2(21.0f, 15.0f),
                                           new Vector2(48.0f, 19.0f),
                                       }.Select(v => v * scale * 2.0f).ToArray());
                case ZoomLevels.Two:
                    return new Circle(Vector2.Zero, 100.0f * scale);
                case ZoomLevels.Three:
                    return new Polygon(CullMode.CullClockwiseFace, new List<Vector2>()
                                       {
                                           new Vector2(125, 61),
                                           new Vector2(114, 68),
                                           new Vector2(89, 73),
                                           new Vector2(34, 73),
                                           new Vector2(27, 64),
                                           new Vector2(34, 54),
                                           new Vector2(89, 51),
                                           new Vector2(114, 55),
                                       }.Select(v => v * scale).ToArray());
                case ZoomLevels.Four:
                    return new Circle(Vector2.Zero, 100.0f * scale);
                case ZoomLevels.Five:
                    return new Circle(Vector2.Zero, 60.0f * scale);

                default: throw new NotImplementedException();
            }
        }


        public static void Initialize(GraphicsDevice gd, ContentManager content)
        {
            EmptySprite = AnimatedSprite.Empty(gd);

            foreach (ZoomLevels zoom in WorldData.AscendingZooms)
            {
                string zoomS = WorldData.ZoomToInt(zoom).ToString();

                WorldBackgrounds[zoom] = content.Load<Texture2D>("Art/Z" + zoomS);

                foreach (Jousting.Jousters jouster in Utilities.OtherFunctions.GetValues<Jousting.Jousters>())
                {
                    string jousterS = (jouster == Jousting.Jousters.Dischord ? "C" : "H");
                    switch (zoom)
                    {
                        case ZoomLevels.One:
                            PlayerSprites[zoom][jouster] = new AnimatedSprite(content.Load<Texture2D>("Art/Player" + jousterS + " " + zoomS),
                                                                              8, TimeSpan.FromSeconds(0.15), true, -1, 1);
                            break;
                        case ZoomLevels.Three:
                            PlayerSprites[zoom][jouster] = new AnimatedSprite(content.Load<Texture2D>("Art/Player" + jousterS + " " + zoomS),
                                                                              14, TimeSpan.FromSeconds(0.1), true, -1, 1);
                            break;
                        default:
                            PlayerSprites[zoom][jouster] = new AnimatedSprite(content.Load<Texture2D>("Art/Player" + jousterS + " " + zoomS));
                            break;
                    }
                    PlayerSprites[zoom][jouster].SetOriginToCenter();
                    PlayerSprites[zoom][jouster].StartAnimation();
                }
            }

            SpecialUIAlert[Jousting.Jousters.Harmony] = new AnimatedSprite(content.Load<Texture2D>("Art/Special H"), 4, TimeSpan.FromSeconds(0.02), true, -1, 1);
            SpecialUIAlert[Jousting.Jousters.Harmony].DrawArgs.Origin = new Vector2(0.0f, SpecialUIAlert[Jousting.Jousters.Harmony].ExactHeight);
            SpecialUIAlert[Jousting.Jousters.Harmony].DrawArgs.Scale *= 2.0f;
            SpecialUIAlert[Jousting.Jousters.Dischord] = new AnimatedSprite(content.Load<Texture2D>("Art/Special C"), 4, TimeSpan.FromSeconds(0.02), true, -1, 1);
            SpecialUIAlert[Jousting.Jousters.Dischord].DrawArgs.Origin = new Vector2(SpecialUIAlert[Jousting.Jousters.Dischord].ExactWidth,
                                                                                     SpecialUIAlert[Jousting.Jousters.Dischord].ExactHeight);
            SpecialUIAlert[Jousting.Jousters.Dischord].DrawArgs.Scale *= 2.0f;


            SpecialUIAlert[Jousting.Jousters.Harmony].StartAnimation();
            SpecialUIAlert[Jousting.Jousters.Dischord].StartAnimation();

            DebugFont = content.Load<SpriteFont>("DebugFont");
            WorldFont = content.Load<SpriteFont>("WorldFont");
        }


        public static void DrawSpecialBar(SpriteBatch sb, float specialAmount, Point windowSize)
        {
            Color backgroundCol = Color.Black;
            backgroundCol.A = 175;

            Color foregroundCol = Color.White;
            foregroundCol.A = (byte)(255.0f *  specialAmount);

            //Draw the background.
            const int border = 50,
                      height = 50;
            Rect backgroundRect = new Rect(border, windowSize.Y - border - height,
                                                     windowSize.X - (2 * border), height);
            TexturePrimitiveDrawer.DrawRect(backgroundRect, sb, backgroundCol, 1);

            //Draw the foreground.
            const int border2 = 10;
            Rect foregroundRect = new Rect(backgroundRect.X + border2, backgroundRect.Y + border2,
                                                     (int)(specialAmount * (backgroundRect.Width - (2 * border2))), backgroundRect.Height - (2 * border2));
            TexturePrimitiveDrawer.DrawRect(foregroundRect, sb, foregroundCol, 1);


            //Draw the UI alerts if the special is ready.
            if (specialAmount == 1.0f)
            {
                const float offsetX = border + 10.0f,
                            offsetY = height + border + 10.0f;
                SpecialUIAlert[Jousting.Jousters.Harmony].Draw(new Vector2(offsetX, windowSize.Y - offsetY), sb);
                SpecialUIAlert[Jousting.Jousters.Dischord].Draw(new Vector2(windowSize.X - offsetX, windowSize.Y - offsetY), sb);
            }
        }
    }
}
