using System;
using System.Collections.Generic;
using Utilities.Graphics;
using Utilities.Math.Shape;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace gamejam2014.Minigames.Minigame_1
{
    public static class ArtAssets1
    {
        public static Shape GoodBacteriaShape(Microsoft.Xna.Framework.Vector2 pos, float zoomScale)
        {
            return new Circle(pos, zoomScale * 15.0f);
        }
        public static Shape InfectedBacteriaShape(Microsoft.Xna.Framework.Vector2 pos, float zoomScale)
        {
            return new Circle(pos, zoomScale * 15.0f);
        }

        public static AnimatedSprite GoodBacteria, InfectedBacteria;
        public static AnimatedSprite Spikes, Aura;

        public static void Initialize(GraphicsDevice device, ContentManager content)
        {
            GoodBacteria = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/Pickup Good"));
            GoodBacteria.SetOriginToCenter();
            GoodBacteria.StartAnimation();

            InfectedBacteria = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/Pickup Infected"));
            InfectedBacteria.SetOriginToCenter();
            InfectedBacteria.StartAnimation();

            Spikes = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/amoeba_spikes"), 8, ArtAssets.PlayerSprites[ZoomLevels.One][Jousting.Jousters.Harmony].FrameLength, true);
            Spikes.SetOriginToCenter();
            Spikes.StartAnimation();
        }
    }
}
