using System;
using System.Collections.Generic;
using gamejam2014.Jousting;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_1
{
    /// <summary>
    /// A blocker that can be infected or not.
    /// </summary>
    public class Bacteria : Blocker
    {
        public bool IsInfected { get; private set; }

        public Bacteria(bool infected, Microsoft.Xna.Framework.Vector2 pos, float zoomScale)
            : base(infected ? ArtAssets1.InfectedBacteria : ArtAssets1.GoodBacteria,
                   infected ? ArtAssets1.InfectedBacteriaShape(pos, zoomScale) : ArtAssets1.GoodBacteriaShape(pos, zoomScale),
                   Single.PositiveInfinity,
                   infected ? PhysicsData1.InfectedBacteriaMass : PhysicsData1.GoodBacteriaMass)
        {
            IsInfected = infected;
            Sprite = new Utilities.Graphics.AnimatedSprite(Sprite);
            Sprite.DrawArgs.Rotation = Utilities.Math.UsefulMath.RandomF(-Microsoft.Xna.Framework.MathHelper.Pi,
                                                                         Microsoft.Xna.Framework.MathHelper.Pi);

            Velocity = zoomScale * new V2(Utilities.Math.UsefulMath.RandomF(-1.0f, 1.0f) * PhysicsData1.BacteriaDriftMaxSpeed,
                                          Utilities.Math.UsefulMath.RandomF(-1.0f, 1.0f) * PhysicsData1.BacteriaDriftMaxSpeed);
        }
    }
}
