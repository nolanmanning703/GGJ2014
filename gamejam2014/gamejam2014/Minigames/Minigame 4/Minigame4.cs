using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamejam2014.Minigames.Minigame_4
{
    public class Minigame4 : Minigame
    {
        private int DiscordScore;
        private int HarmonyScore;
        private List<String> BadStrings = new List<String>()
        {
            "Television",
            "Facebook",
            "Office Politics",
            "Men"
        };

        private List<String> GoodStrings = new List<String>()
        {
            "Philosophy",
            "The Environment",
            "World Events",
            "Art"
        };

        public Minigame4(ZoomLevels zoom)
            : base(zoom)
        {
        }

        protected override void Update(Jousting.Jouster.CollisionData playerCollision)
        {
      
        }
        protected override void Reset()
        {
            
        }

        protected override void DrawAbovePlayers(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);

            sb.DrawString(ArtAssets.WorldFont, "Blah", Vector2.Zero, Color.White, 0.0f, Vector2.Zero, WorldData.ZoomScaleAmount[World.CurrentZoom], SpriteEffects.None, 1.0f);

            sb.End();
        } 
        

    }
}
