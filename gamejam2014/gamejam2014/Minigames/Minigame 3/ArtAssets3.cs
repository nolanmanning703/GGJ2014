using System;
using System.Linq;
using System.Collections.Generic;
using Utilities.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_3
{
    public static class ArtAssets3
    {
        public static Utilities.Math.Shape.Shape GetDoghouseShape(float zoomScale, V2 pos)
        {
            return new Utilities.Math.Shape.Polygon(Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace,
                                                    new List<V2>()
                                                    {
                                                        new V2(1.0f),
                                                        new V2(0.0f, -1.0f),
                                                        new V2(0.0f, 1.0f),
                                                    }.Select(v => (v * zoomScale) + pos).ToArray());
        }
        public static float HillRadius = 50.0f;

        public static AnimatedSprite HillSprite;
        public static AnimatedSprite DogHouseSprite;

        public static void Initialize(GraphicsDevice device, ContentManager content)
        {
            HillSprite = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/Hill"), 4, TimeSpan.FromSeconds(0.05), true, -1, 1);
            HillSprite.SetOriginToCenter();
            HillSprite.StartAnimation();
        }
    }
}
