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

        private float timeSinceParticles = 0.0f;

        public Bacteria(Microsoft.Xna.Framework.Vector2 pos, float zoomScale)
            : base(ArtAssets1.GoodBacteria, ArtAssets1.GoodBacteriaShape(pos, zoomScale), false, Single.PositiveInfinity, PhysicsData1.GoodBacteriaMass)
        {
            IsInfected = false;

            Sprite = new Utilities.Graphics.AnimatedSprite(Sprite);
            Sprite.DrawArgs.Rotation = Utilities.Math.UsefulMath.RandomF(-Microsoft.Xna.Framework.MathHelper.Pi,
                                                                         Microsoft.Xna.Framework.MathHelper.Pi);
            Rotation = Sprite.DrawArgs.Rotation;

            Velocity = zoomScale * new V2(Utilities.Math.UsefulMath.RandomF(-1.0f, 1.0f) * PhysicsData1.BacteriaDriftMaxSpeed,
                                          Utilities.Math.UsefulMath.RandomF(-1.0f, 1.0f) * PhysicsData1.BacteriaDriftMaxSpeed);
        }

        public void Infect(float zoomScale)
        {
            if (!IsInfected)
            {
                WorldData.Minigames[ZoomLevels.One].AdditiveParticles.Merge(ParticleAssets1.GetInfectParticles(KarmaWorld.World.CurrentTime, Pos));

                IsInfected = true;

                Sprite = ArtAssets1.InfectedBacteria;

                float rot = Rotation;
                ColShape = ArtAssets1.InfectedBacteriaShape(Pos, zoomScale);
                Rotation = rot;

                Mass = PhysicsData1.InfectedBacteriaMass;
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gt)
        {
            if (IsInfected)
            {
                if (timeSinceParticles > ParticleAssets1.BadParticleSpawnInterval)
                {
                    timeSinceParticles = 0.0f;
                    WorldData.Minigames[ZoomLevels.One].AlphaParticles.Merge(ParticleAssets1.GetBacteriaParticles(gt, Pos, true));
                }
            }
            else
            {
                if (timeSinceParticles > ParticleAssets1.GoodParticleSpawnInterval)
                {
                    timeSinceParticles = 0.0f;
                    WorldData.Minigames[ZoomLevels.One].AlphaParticles.Merge(ParticleAssets1.GetBacteriaParticles(gt, Pos, false));
                }
            }


            timeSinceParticles += (float)gt.ElapsedGameTime.TotalSeconds;

            base.Update(gt);
        }
    }
}
