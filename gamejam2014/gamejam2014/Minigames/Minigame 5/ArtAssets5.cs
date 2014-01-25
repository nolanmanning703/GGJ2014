using System;
using System.Collections.Generic;
using Utilities.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace gamejam2014.Minigames.Minigame_5
{
    public static class ArtAssets5
    {
        public static AnimatedSprite BlackHole;

        public static void Initialize(GraphicsDevice device, ContentManager content)
        {
            BlackHole = new AnimatedSprite(content.Load<Texture2D>("Art/Z5 Art/Black Hole"));
            BlackHole.SetOriginToCenter();
        }
    }
}
