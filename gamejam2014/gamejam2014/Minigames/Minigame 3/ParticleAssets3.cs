using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Graphics;

namespace gamejam2014.Minigames.Minigame_3
{
    public static class ParticleAssets3
    {
        private static float Scale { get { return WorldData.ZoomScaleAmount[ZoomLevels.Three]; } }

        public static AnimatedSprite DirtCloud;


        public static void Initialize(GraphicsDevice gd, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            DirtCloud = new AnimatedSprite(content.Load<Texture2D>("Art/Z3 Art/Dirt Particle"));
            DirtCloud.DrawArgs.Scale = new Vector2(Scale * 0.25f);
            DirtCloud.SetOriginToCenter();
            DirtCloud.StartAnimation();
        }

        //public static ParticleEffect
    }
}
