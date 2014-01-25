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
        public Minigame5(ZoomLevels zoom)
            : base(zoom)
        {

        }

        protected override void Reset()
        {
            
        }

        protected override void Update(Jousting.Jouster.CollisionData playerCollision)
        {
            Utilities.Math.Shape.Circle blackHole = PhysicsData5.GetBlackHole();
            V2 harmonyToBH = UsefulMath.FindDirection(Harmony.Pos, blackHole.Center, false);
            V2 dischordToBH = UsefulMath.FindDirection(Dischord.Pos, blackHole.Center, false);

            float scale = WorldData.ZoomScaleAmount[World.CurrentZoom];

            if (blackHole.Touches(Harmony.ColShape))
            {
                Harmony.Velocity = scale * PhysicsData5.BlackHolePushBack * -V2.Normalize(harmonyToBH);
                //Harmony.Update(World.CurrentTime);
            }
            if (blackHole.Touches(Dischord.ColShape))
            {
                Dischord.Velocity = scale * PhysicsData5.BlackHolePushBack * -V2.Normalize(dischordToBH);
                //Dischord.Update(World.CurrentTime);
            }

            Harmony.Acceleration = V2.Normalize(harmonyToBH) * PhysicsData5.GetBlackHolePull(harmonyToBH.Length(), scale);
            Dischord.Acceleration = V2.Normalize(dischordToBH) * PhysicsData5.GetBlackHolePull(dischordToBH.Length(), scale);

            ArtAssets5.BlackHole.UpdateAnimation(World.CurrentTime);
        }

        protected override void DrawBelowPlayers(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);
            ArtAssets5.BlackHole.DrawArgs.Scale *= WorldData.ZoomScaleAmount[ZoomLevels.Five];
            ArtAssets5.BlackHole.Draw(PhysicsData5.GetBlackHole().Center, sb);
            ArtAssets5.BlackHole.DrawArgs.Scale /= WorldData.ZoomScaleAmount[ZoomLevels.Five];
            sb.End();
        }
        protected override void DrawAbovePlayers(SpriteBatch sb)
        {
            sb.Begin();
            sb.DrawString(ArtAssets.DebugFont, Harmony.Acceleration.ToString(), new V2(40.0f, 200.0f), Microsoft.Xna.Framework.Color.White);
            sb.End();
        }
    }
}
