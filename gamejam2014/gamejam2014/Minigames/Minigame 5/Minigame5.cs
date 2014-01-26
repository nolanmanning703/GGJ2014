using System;
using System.Collections.Generic;
using Utilities.Math;
using Microsoft.Xna.Framework.Graphics;
using gamejam2014;
using gamejam2014.Minigames;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_5
{
    public class Minigame5 : Minigame
    {
        public Jousting.Blocker BlackHole;
        public Utilities.Math.Shape.Circle BlackHoleCol { get { return (Utilities.Math.Shape.Circle)BlackHole.ColShape; } }

        public Minigame5(ZoomLevels zoom)
            : base(zoom)
        {

        }

        protected override void Reset()
        {
            Blockers.Add(new Jousting.Blocker(ArtAssets5.BlackHole, PhysicsData5.GetBlackHole(), 0.0f, 1.0f));
            Blockers[0].OnHitByJouster += (s, e) =>
            {
                e.Enemy.Velocity = WorldData.ZoomScaleAmount[World.CurrentZoom] * PhysicsData5.BlackHolePushBack * V2.Normalize(UsefulMath.FindDirection(e.Enemy.Pos, BlackHole.Pos, false));
                e.Enemy.Health -= PhysicsData5.BlackHoleDamage;
            };
            BlackHole = Blockers[0];
        }

        protected override void Update(Jousting.Jouster.CollisionData playerCollision)
        {
            Utilities.Math.Shape.Circle blackHole = BlackHoleCol;
            V2 harmonyToBH = UsefulMath.FindDirection(Harmony.Pos, blackHole.Center, false);
            V2 dischordToBH = UsefulMath.FindDirection(Dischord.Pos, blackHole.Center, false);

            float scale = WorldData.ZoomScaleAmount[World.CurrentZoom];


            Harmony.Acceleration = V2.Normalize(harmonyToBH) * PhysicsData5.GetBlackHolePull(harmonyToBH.Length(), scale);
            Dischord.Acceleration = V2.Normalize(dischordToBH) * PhysicsData5.GetBlackHolePull(dischordToBH.Length(), scale);
            foreach (Jousting.Blocker blocker in Blockers)
            {
                blocker.Acceleration = UsefulMath.FindDirection(blocker.Pos, blackHole.Center) *
                                       PhysicsData5.GetBlackHolePull((blocker.Pos - blackHole.Center).Length(), scale);

            }
        }

        protected override void OnMinigameEnd()
        {
            Harmony.Rotation = 0.0f;
            HarmonySprite.DrawArgs.Rotation = 0.0f;
        }

        public override void OnHarmonySpecial()
        {

        }
        public override void OnDischordSpecial()
        {

        }

        protected override void DrawAbovePlayers(SpriteBatch sb)
        {
            if ((World.CurrentZoom == WorldData.ZoomIn(ZoomLevels.Five) && World.ZoomingIn) ||
                (World.CurrentZoom == ZoomLevels.Five && World.ZoomingOut))
            {
                //Draw the rendered world on top of the globe.
                sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, World.CamTransform);
                sb.Draw(World.RenderedWorldTex, Harmony.Pos, null, Microsoft.Xna.Framework.Color.White, 0.0f,
                        0.5f * new V2(World.RenderedWorldTex.Width, World.RenderedWorldTex.Height), 1.0f, SpriteEffects.None, 1.0f);
                sb.End();
            }
        }
    }
}
