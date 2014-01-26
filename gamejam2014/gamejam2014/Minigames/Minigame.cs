using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Utilities;
using gamejam2014.Jousting;

namespace gamejam2014.Minigames
{
    /// <summary>
    /// A single zoom level game.
    /// </summary>
    public abstract class Minigame
    {
        protected KarmaWorld World { get { return KarmaWorld.World; } }
        protected Microsoft.Xna.Framework.GameTime Time { get { return KarmaWorld.World.CurrentTime; } }

        /// <summary>
        /// If true, this game has ended and should move upwards to the minigame above this one.
        /// </summary>
        public bool MoveUp { get; set; }
        /// <summary>
        /// If true, this game has ended and should move downwards to the minigame below this one.
        /// </summary>
        public bool MoveDown { get; set; }

        public ZoomLevels CurrentZoom;

        public Jouster Harmony, Dischord;
        public List<Blocker> Blockers = new List<Blocker>();
        private List<Blocker> ToRemove = new List<Blocker>();

        public TimerManager Timers = new TimerManager();

        public float TimeSinceMinigameStart { get; private set; }

        protected Utilities.Graphics.AnimatedSprite HarmonySprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Harmony]; } }
        protected Utilities.Graphics.AnimatedSprite DischordSprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Dischord]; } }

        private List<Utilities.Graphics.AnimatedSprite> UniqueBlockerSprites = new List<Utilities.Graphics.AnimatedSprite>();


        public Minigame(ZoomLevels currentZoom)
        {
            CurrentZoom = currentZoom;
            MoveUp = false;
            MoveDown = false;

            TimeSinceMinigameStart = 0.0f;
        }

        public void ResetGame()
        {
            MoveUp = false;
            MoveDown = false;

            TimeSinceMinigameStart = 0.0f;

            Harmony = new Jousting.Jouster(Jousting.Jousters.Harmony,
                                           WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Harmony),
                                           CurrentZoom);
            Dischord = new Jousting.Jouster(Jousting.Jousters.Dischord,
                                            WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Dischord),
                                            CurrentZoom);
            Blockers.Clear();
            Timers.ClearTimers();

            Reset();
        }
        public void UpdateGame()
        {
            Timers.Update(World.CurrentTime);
            TimeSinceMinigameStart += (float)World.CurrentTime.ElapsedGameTime.TotalSeconds;

            //Update players.
            Harmony.Update(World.CurrentTime);
            Dischord.Update(World.CurrentTime);
            Jousting.Jouster.CollisionData colDat = Jousting.Jouster.CheckCollision(Harmony, Dischord);

            if (Harmony.Health <= 0.0f)
            {
                MoveDown = true;
            }
            if (Dischord.Health <= 0.0f)
            {
                MoveUp = true;
            }

            //Remove dead blockers..
            foreach (Blocker b in ToRemove)
                Blockers.Remove(b);
            ToRemove.Clear();

            //Update blockers.
            UniqueBlockerSprites.Clear();
            for (int i = 0; i < Blockers.Count; ++i)
            {
                Blockers[i].Update(World.CurrentTime);

                if (!UniqueBlockerSprites.Contains(Blockers[i].Sprite))
                    UniqueBlockerSprites.Add(Blockers[i].Sprite);
            }
            //Blocker/Blocker and Blocker/Jouster collision.
            for (int i = 0; i < Blockers.Count; ++i)
            {
                for (int j = i + 1; j < Blockers.Count; ++j)
                {
                    Blocker.CheckCollision(Blockers[i], Blockers[j]);
                }
                Blocker.CheckCollision(Blockers[i], Harmony);
                Blocker.CheckCollision(Blockers[i], Dischord);
            }

            //Update animation.
            HarmonySprite.UpdateAnimation(World.CurrentTime);
            DischordSprite.UpdateAnimation(World.CurrentTime);
            foreach (Utilities.Graphics.AnimatedSprite sprite in UniqueBlockerSprites)
            {
                sprite.UpdateAnimation(World.CurrentTime);
            }

            //Update minigame logic.
            Update(colDat);

            if (MoveUp || MoveDown)
            {
                OnMinigameEnd();
            }
        }
        public void Draw(SpriteBatch sb)
        {
            DrawBelowPlayers(sb);

            float scale = WorldData.ZoomScaleAmount[CurrentZoom],
                  invScale = 1.0f / scale;

            HarmonySprite.DrawArgs.Rotation = Harmony.Rotation;
            HarmonySprite.DrawArgs.Scale *= scale;
            if (!MoveUp && !MoveDown) HarmonySprite.DrawArgs.Color = new Microsoft.Xna.Framework.Color(Harmony.Health, Harmony.Health, Harmony.Health);
            else HarmonySprite.DrawArgs.Color = Microsoft.Xna.Framework.Color.White;
            DischordSprite.DrawArgs.Rotation = Dischord.Rotation;
            DischordSprite.DrawArgs.Scale *= scale;
            if (!MoveUp && !MoveDown) DischordSprite.DrawArgs.Color = new Microsoft.Xna.Framework.Color(Dischord.Health, Dischord.Health, Dischord.Health);
            else DischordSprite.DrawArgs.Color = Microsoft.Xna.Framework.Color.White;

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);

            for (int i = 0; i < Blockers.Count; ++i) if (!Blockers[i].IsAbovePlayer)
            {
                Blockers[i].Sprite.DrawArgs.Scale *= scale;
                Blockers[i].Sprite.Draw(Blockers[i].Pos, sb);
                Blockers[i].Sprite.DrawArgs.Scale *= invScale;
            }
            HarmonySprite.Draw(Harmony.Pos, sb);
            DischordSprite.Draw(Dischord.Pos, sb);
            for (int i = 0; i < Blockers.Count; ++i) if (Blockers[i].IsAbovePlayer)
            {
                Blockers[i].Sprite.DrawArgs.Scale *= scale;
                Blockers[i].Sprite.Draw(Blockers[i].Pos, sb);
                Blockers[i].Sprite.DrawArgs.Scale *= invScale;
            }

            sb.End();

            HarmonySprite.DrawArgs.Scale *= invScale;
            DischordSprite.DrawArgs.Scale *= invScale;

            DrawAbovePlayers(sb);
        }

        /// <summary>
        /// Queues the given blocker to be destroyed next Update() call.
        /// </summary>
        public void KillBlocker(Blocker b)
        {
            ToRemove.Add(b);
        }

        /// <summary>
        /// Call this to activate Harmony's special.
        /// </summary>
        public abstract void OnHarmonySpecial();
        /// <summary>
        /// Call this to activate Dischord's special.
        /// </summary>
        public abstract void OnDischordSpecial();


        protected virtual void OnMinigameEnd() { }

        protected abstract void Reset();
        protected abstract void Update(Jousting.Jouster.CollisionData playerCollision);

        protected virtual void DrawBelowPlayers(SpriteBatch sb) { }
        protected virtual void DrawAbovePlayers(SpriteBatch sb) { }
    }
}
