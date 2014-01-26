using System;
using System.Linq;
using System.Collections.Generic;
using Utilities.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_5
{
    public static class ArtAssets5
    {
        public static Utilities.Math.Shape.Shape GetRingShape(V2 pos, float zoomScale)
        {
            return new Utilities.Math.Shape.Polygon(CullMode.CullClockwiseFace, new List<V2>()
            {
                new V2(35, 128),
                new V2(33, 181),
                new V2(31, 214),
                new V2(26, 240),
                new V2(18, 253.5f),

                new V2(9, 240),
                new V2(4, 214),
                new V2(1, 181),
                new V2(0, 128),

                new V2(1, 58),
                new V2(4, 41),
                new V2(9, 16),
                new V2(18, 0),

                new V2(26, 16),
                new V2(31, 41),
                new V2(33, 58),
            }.Select(v => (zoomScale * v) + pos).ToArray());
        }

        public static AnimatedSprite BlackHole;
        public static AnimatedSprite Ring;

        public static void Initialize(GraphicsDevice device, ContentManager content)
        {
            BlackHole = new AnimatedSprite(content.Load<Texture2D>("Art/Z5 Art/Black Hole"), 6, TimeSpan.FromSeconds(0.1), true);
            BlackHole.SetOriginToCenter();
            BlackHole.StartAnimation();

            Ring = new AnimatedSprite(content.Load<Texture2D>("Art/Z5 Art/Ring"));
            Ring.SetOriginToCenter();
            Ring.StartAnimation();
        }
    }
}
