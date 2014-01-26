using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Graphics;

namespace gamejam2014.Minigames.Minigame_1
{
    public static class ParticleAssets1
    {
        private static float Scale { get { return WorldData.ZoomScaleAmount[ZoomLevels.One]; } }

        private static AnimatedSprite GoodParticle, BadParticle,
                                      CollectGoodParticle, CollectBadParticle,
                                      InfectBacteriaParticle;
        public static float GoodParticleSpawnInterval = 0.01f,
                            BadParticleSpawnInterval = 0.01f;

        public static void Initialize(GraphicsDevice gd, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            GoodParticle = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/GoodParticle"));
            GoodParticle.SetOriginToCenter();
            GoodParticle.StartAnimation();
            GoodParticle.DrawArgs.Scale = new Vector2(Scale * 0.25f);

            BadParticle = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/InfectedParticle"));
            BadParticle.SetOriginToCenter();
            BadParticle.StartAnimation();
            BadParticle.DrawArgs.Scale = new Vector2(Scale * 0.25f);

            CollectGoodParticle = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/Collected Particle Good"));
            CollectGoodParticle.SetOriginToCenter();
            CollectGoodParticle.DrawArgs.Scale = new Vector2(Scale);

            CollectBadParticle = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/Collected Particle Bad"));
            CollectBadParticle.SetOriginToCenter();
            CollectBadParticle.DrawArgs.Scale = new Vector2(Scale);

            InfectBacteriaParticle = new AnimatedSprite(content.Load<Texture2D>("Art/Z1 Art/MadeInfectedParticle"));
            InfectBacteriaParticle.SetOriginToCenter();
            InfectBacteriaParticle.DrawArgs.Scale = new Vector2(Scale * 0.65f);
        }

        public static ParticleEffect GetBacteriaParticles(GameTime gt, Vector2 pos, bool isInfected)
        {
            return new ParticleEffect(gt, ArtAssets.EmptySprite, (isInfected ? BadParticle : GoodParticle),
                                      Vector2.Zero, pos, 1, false, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.1),
                                      15.0f * Scale, Vector2.Zero, Vector2.Zero,
                                      0.0f, 5.0f * Scale, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                      true, true);
        }
        public static ParticleEffect GetCollectionParticles(GameTime gt, Vector2 pos, bool isInfected)
        {
            return new ParticleEffect(gt, ArtAssets.EmptySprite, (isInfected ? CollectBadParticle : CollectGoodParticle),
                                      Vector2.Zero, pos, 20, false, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.5), Scale * 30.0f,
                                      Vector2.Zero, Vector2.Zero, 0.0f, 2.0f, 0.0f, 0.0f, 100.0f * Scale, 0.0f, 0.0f, -200.0f * Scale, 0.0f, 0.0f, true, true);
        }
        public static ParticleEffect GetInfectParticles(GameTime gt, Vector2 pos)
        {
            return new ParticleEffect(gt, ArtAssets.EmptySprite, InfectBacteriaParticle,
                                      Vector2.Zero, pos, 5, false, TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.1),
                                      Scale * 5.0f, Vector2.Zero, Vector2.Zero, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, true);
        }
    }
}
