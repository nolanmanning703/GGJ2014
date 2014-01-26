using System;
using System.Linq;
using System.Collections.Generic;
using Utilities.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using V2 = Microsoft.Xna.Framework.Vector2;
using Col = Microsoft.Xna.Framework.Color;

namespace gamejam2014.Minigames.Minigame_3
{
    public static class ArtAssets3
    {
        public static Utilities.Math.Shape.Shape GetDoghouseShape(float zoomScale, V2 pos)
        {
            Utilities.Math.Shape.Shape s = new Utilities.Math.Shape.Polygon(Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace,
                                                                            new List<V2>()
                                                                            {
                                                                                new V2(64, 18),
                                                                                new V2(112, 25.5f),
                                                                                new V2(112, 99.5f),
                                                                                new V2(64, 106),
                                                                                new V2(15, 99.5f),
                                                                                new V2(15, 25.5f),
                                                                            }.Select(v => (v * zoomScale * 2f) + (pos * 2f)).ToArray());
            s.Center = pos;
            return s;
        }
        public static float HillRadius = 100.0f;

        public static Utilities.Math.Shape.Shape GetTennisBallShape(float zoomScale, V2 pos)
        {
            return new Utilities.Math.Shape.Circle(zoomScale * pos, zoomScale * 8.0f);
        }


        public static AnimatedSprite HarmonyJousterStill, DischordJousterStill;

        public static AnimatedSprite HillSprite;
        public static AnimatedSprite DogHouseSprite;
        public static AnimatedSprite DogHouseLowerSprite;
        public static AnimatedSprite ConfusedSprite;
        public static AnimatedSprite TennisBall;

        public static void Initialize(GraphicsDevice device, ContentManager content)
        {
            HillSprite = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/Hill"));
            HillSprite.SetOriginToCenter();
            HillSprite.StartAnimation();

            ConfusedSprite = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/Stunned"), 2, TimeSpan.FromSeconds(0.25), true, -1, 0);
            ConfusedSprite.SetOriginToCenter();
            ConfusedSprite.StartAnimation();

            DogHouseSprite = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/doghouse"));
            DogHouseSprite.SetOriginToCenter();
            DogHouseLowerSprite = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/doghouse lower"));
            DogHouseLowerSprite.SetOriginToCenter();

            HarmonyJousterStill = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/dog_still"), 8, TimeSpan.FromSeconds(0.1), true);
            HarmonyJousterStill.SetOriginToCenter();
            HarmonyJousterStill.StartAnimation();

            DischordJousterStill = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/baddog_still"), 8, TimeSpan.FromSeconds(0.1), true);
            DischordJousterStill.SetOriginToCenter();
            DischordJousterStill.StartAnimation();

            TennisBall = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/tennisball"));
            TennisBall.SetOriginToCenter();
            TennisBall.StartAnimation();
        }

        public static void DrawHillTimeBar(V2 playerPos, float playerTimeInHill, SpriteBatch sb)
        {
            float lerp = playerTimeInHill / PhysicsData3.TimeInHillToWin;

            V2 offset = new V2(-50.0f, -50.0f);

            Col backgroundCol = Col.Black;
            const int width = 20, height = 100;
            TexturePrimitiveDrawer.DrawRect(new Microsoft.Xna.Framework.Rectangle((int)(playerPos.X + offset.X) - width,
                                                                                  (int)(playerPos.Y + offset.Y) - height,
                                                                                  width, height),
                                            sb, backgroundCol, 1);

            Col foregroundCol = Col.White;
            foregroundCol.A = (byte)(255.0f * lerp);
            const int border = 5;
            int height2 = (byte)((height - (border * 2)) * lerp);
            TexturePrimitiveDrawer.DrawRect(new Microsoft.Xna.Framework.Rectangle((int)(playerPos.X + offset.X) - width + border,
                                                                                  (int)(playerPos.Y + offset.Y) - height2,
                                                                                  width - (2 * border),
                                                                                  height2 - border),
                                            sb, foregroundCol, 1);
        }
    }
}
