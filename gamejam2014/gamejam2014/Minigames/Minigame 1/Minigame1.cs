using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using Utilities.Math;
using Utilities.Graphics;
using gamejam2014.Jousting;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Minigames.Minigame_1
{
    public class Minigame1 : Minigame
    {
        public Minigame1(ZoomLevels zoom)
            : base(zoom)
        {

        }


        /// <summary>
        /// Resizes the given jouster by changing his mass by the given amount.
        /// </summary>
        private void ResizeJouster(float massChange, Jouster jouster)
        {
            jouster.Mass += massChange;

            float scale = jouster.Mass / PhysicsData.JousterStartingMass[ZoomLevels.One];

            ArtAssets.PlayerSprites[World.CurrentZoom][jouster.ThisJouster].DrawArgs.Scale = V2.One * scale;

            V2 oldPos = jouster.Pos;
            float oldRot = jouster.Rotation;
            jouster.ColShape = ArtAssets.GetJousterShape(ZoomLevels.One, scale * WorldData.ZoomScaleAmount[ZoomLevels.One]);
            jouster.Pos = oldPos;
            jouster.Rotation = oldRot;
        }

        /// <summary>
        /// Returns whether the given position is inside either Jouster or a Blocker.
        /// </summary>
        private bool IsInvalid(V2 pos)
        {
            return Harmony.ColShape.PosInside(pos) ||
                   Dischord.ColShape.PosInside(pos) ||
                   Blockers.Any(b => b.ColShape.PosInside(pos));
        }
        /// <summary>
        /// Creates a bacteria at a random valid position.
        /// </summary>
        private Bacteria MakeRandomBacteria()
        {
            Utilities.Math.Shape.Rectangle bounds = World.WorldBounds;
            Interval xEnds = bounds.XEnds,
                     yEnds = bounds.YEnds;

            V2 pos = new V2(xEnds.Random(), yEnds.Random());
            while (IsInvalid(pos))
            {
                pos.X = xEnds.Random();
                pos.Y = yEnds.Random();
            }

            Bacteria bact = new Bacteria(pos, WorldData.ZoomScaleAmount[CurrentZoom]);
            bact.OnHitByJouster += (s, e) =>
                {
                    Bacteria b = s as Bacteria;
                    if (b == null) return;

                    if (e.Enemy.ThisJouster == Jousters.Dischord)
                    {
                        if (!b.IsInfected) b.Infect(WorldData.ZoomScaleAmount[ZoomLevels.One]);
                        e.Enemy.Velocity *= PhysicsData1.BacteriaInfectEnergyDamp;
                    }
                    else
                    {
                        if (b.IsInfected)
                        {
                            ResizeJouster(-PhysicsData1.BacteriaMassGiveToPlayerScale * b.Mass, e.Enemy);
                        }
                        else
                        {
                            ResizeJouster(PhysicsData1.BacteriaMassGiveToPlayerScale * b.Mass, e.Enemy);
                        }
                        if (e.Enemy.Mass <= 0.0f)
                        {
                            e.Enemy.Mass = 1.0f;
                            MoveDown = true;
                            return;
                        }

                        KillBlocker(b);
                    }
                };

            return bact;
        }

        protected override void Reset()
        {
            ArtAssets.PlayerSprites[ZoomLevels.One][Jousters.Harmony].DrawArgs.Scale = V2.One;
            ArtAssets.PlayerSprites[ZoomLevels.One][Jousters.Dischord].DrawArgs.Scale = V2.One;

            Blockers.Add(MakeRandomBacteria());

            //Start the timer loop.
            IntervalCounter bacteriaSpawn = new IntervalCounter(TimeSpan.FromSeconds(PhysicsData1.BacteriaAppearanceTime));
            bacteriaSpawn.IntervalTrigger += (s, e) =>
                {
                    Bacteria bactt = MakeRandomBacteria();
                    Blockers.Add(bactt);

                    IntervalCounter counter = new IntervalCounter(TimeSpan.FromSeconds(PhysicsData1.BacteriaLifeLength));
                    counter.IntervalTrigger += (s2, e2) =>
                        {
                            KillBlocker(bactt);
                        };
                    Timers.AddTimerNextUpdate(counter, true);
                };
            Timers.AddTimerNextUpdate(bacteriaSpawn, false);
        }

        protected override void Update(Jouster.CollisionData playerCollision)
        {
            if (Dischord.IsSpiky_Aura)
            {
                //The aura can infect bacteria at a distance.
                for (int i = 0; i < Blockers.Count; ++i)
                {
                    if (V2.Distance(Blockers[i].Pos, Dischord.ColShape.ClosestOnEdgeToPoint(Blockers[i].Pos)) <
                        PhysicsData1.GetAuraDist(WorldData.ZoomScaleAmount[CurrentZoom]))
                    {
                        ((Bacteria)Blockers[i]).Infect(WorldData.ZoomScaleAmount[CurrentZoom]);
                    }
                }
            }

            ArtAssets1.Spikes.UpdateAnimation(World.CurrentTime);
        }

        public override void OnHarmonySpecial()
        {
            Harmony.IsSpiky_Aura = true;
            IntervalCounter ic = new IntervalCounter(TimeSpan.FromSeconds(PhysicsData1.SpikePowerLength));
            ic.IntervalTrigger += (s, e) =>
                {
                    Harmony.IsSpiky_Aura = false;
                };
            World.Timers.AddTimerNextUpdate(ic, true);
        }
        public override void OnDischordSpecial()
        {
            Dischord.IsSpiky_Aura = true;
            IntervalCounter ic = new IntervalCounter(TimeSpan.FromSeconds(PhysicsData1.AuraPowerLength));
            ic.IntervalTrigger += (s, e) =>
                {
                    Dischord.IsSpiky_Aura = false;
                };
            World.Timers.AddTimerNextUpdate(ic, true);
        }

        protected override void DrawBelowPlayers(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            bool willDraw = Harmony.IsSpiky_Aura || Dischord.IsSpiky_Aura;
            if (!willDraw) return;


            sb.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate, Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied,
                     null, null, null, null, World.CamTransform);

            if (Harmony.IsSpiky_Aura)
            {
                ArtAssets1.Spikes.DrawArgs.Scale = HarmonySprite.DrawArgs.Scale;
                ArtAssets1.Spikes.DrawArgs.Scale *= WorldData.ZoomScaleAmount[ZoomLevels.One];
                ArtAssets1.Spikes.DrawArgs.Rotation = Harmony.Rotation;
                ArtAssets1.Spikes.Draw(Harmony.Pos, sb);
                ArtAssets1.Spikes.DrawArgs.Scale /= WorldData.ZoomScaleAmount[ZoomLevels.One];
            }
            if (Dischord.IsSpiky_Aura)
            {

            }

            sb.End();
        }
    }
}
