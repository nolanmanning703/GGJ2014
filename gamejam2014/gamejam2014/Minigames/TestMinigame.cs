using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gamejam2014.Minigames
{
    /// <summary>
    /// A tester minigame.
    /// </summary>
    public class TestMinigame : Minigame
    {
        public TestMinigame(ZoomLevels currentZoom)
            : base(currentZoom)
        {

        }

        protected override void Reset()
        {
            float scale = WorldData.ZoomScaleAmount[World.CurrentZoom];


            Blockers.Add(new Jousting.Blocker(new Utilities.Graphics.AnimatedSprite(Minigame_5.ArtAssets5.BlackHole),
                         new Utilities.Math.Shape.Circle(Microsoft.Xna.Framework.Vector2.Zero, scale * 10.0f),
                         9999.0f));
            Blockers.Add(new Jousting.Blocker(new Utilities.Graphics.AnimatedSprite(Minigame_5.ArtAssets5.BlackHole),
                         new Utilities.Math.Shape.Circle(scale * new Microsoft.Xna.Framework.Vector2(-200), scale * 10.0f),
                         9999.0f));
            Blockers[0].Sprite.DrawArgs.Scale *= 0.33333f;
            Blockers[1].Sprite.DrawArgs.Scale *= 0.33333f;
            Blockers[1].Velocity = scale * new Microsoft.Xna.Framework.Vector2(500);
        }
        protected override void Update(Jousting.Jouster.CollisionData playerCollision)
        {

        }
    }
}
