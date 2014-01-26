﻿using System;
using System.Collections.Generic;
using Utilities.Math;
using Utilities.Math.Shape;
using Utilities.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gamejam2014.Minigames.Minigame_3
{
    public class Minigame3 : Minigame
    {
        public float TimeInHill;
        public Circle HillShape;
        public bool HarmonyInsideHill;

        public Minigame3(ZoomLevels zoom)
            : base(zoom)
        {
            TimeInHill = 0.0f;
            HarmonyInsideHill = false;
            HillShape = null;
        }

        protected override void Reset()
        {
            TimeInHill = 0.0f;
            HarmonyInsideHill = false;
            HillShape = PhysicsData3.GetHillCircle(WorldData.ZoomScaleAmount[CurrentZoom]);
        }

        protected override void Update(Jousting.Jouster.CollisionData playerCollision)
        {
            HarmonyInsideHill = HillShape.Touches(Harmony.ColShape);
            if (HarmonyInsideHill)
            {
                TimeInHill += (float)World.CurrentTime.ElapsedGameTime.TotalSeconds;
            }

            if (TimeInHill > PhysicsData3.TimeInHillToWin)
            {
                MoveUp = true;
                return;
            }

            ArtAssets3.HillSprite.UpdateAnimation(World.CurrentTime);
            ArtAssets3.ConfusedSprite.UpdateAnimation(World.CurrentTime);
        }

        public override void OnHarmonySpecial()
        {
            Dischord.IsStunned = true;

            Utilities.IntervalCounter counter = new Utilities.IntervalCounter(TimeSpan.FromSeconds(PhysicsData3.SpecialStunTime));
            counter.IntervalTrigger += (s, e) =>
                {
                    Dischord.IsStunned = false;
                };
            World.Timers.AddTimerNextUpdate(counter, true);
        }
        public override void OnDischordSpecial()
        {
            Harmony.IsStunned = true;

            Utilities.IntervalCounter counter = new Utilities.IntervalCounter(TimeSpan.FromSeconds(PhysicsData3.SpecialStunTime));
            counter.IntervalTrigger += (s, e) =>
            {
                Harmony.IsStunned = false;
            };
            World.Timers.AddTimerNextUpdate(counter, true);
        }

        protected override void DrawBelowPlayers(SpriteBatch sb)
        {
            if (HarmonyInsideHill)
                ArtAssets3.HillSprite.DrawArgs.Color = Color.White;
            else ArtAssets3.HillSprite.DrawArgs.Color = Color.DarkGray;

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);
            ArtAssets3.HillSprite.Draw(HillShape.Center, sb);
            sb.End();
        }
        protected override void DrawAbovePlayers(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);
            if (Harmony.IsStunned)
            {
                ArtAssets3.ConfusedSprite.Draw(Harmony.Pos, sb);
            }
            if (Dischord.IsStunned)
            {
                ArtAssets3.ConfusedSprite.Draw(Dischord.Pos, sb);
            }
            ArtAssets3.DrawHillTimeBar(Harmony.Pos, TimeInHill, sb);
            sb.End();
        }
    }
}
