using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
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
        public List<Blocker> Blockers;

        protected Utilities.Graphics.AnimatedSprite HarmonySprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Harmony]; } }
        protected Utilities.Graphics.AnimatedSprite DischordSprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Dischord]; } }

        private List<Utilities.Graphics.AnimatedSprite> UniqueBlockerSprites = new List<Utilities.Graphics.AnimatedSprite>();


        public Minigame(ZoomLevels currentZoom) { CurrentZoom = currentZoom;  MoveUp = false; MoveDown = false; }

        public void ResetGame()
        {
            MoveUp = false;
            MoveDown = false;

            Harmony = new Jousting.Jouster(Jousting.Jousters.Harmony,
                                           WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Harmony));
            Dischord = new Jousting.Jouster(Jousting.Jousters.Dischord,
                                            WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Dischord));
            Blockers = new List<Blocker>();

            Reset();
        }
        public void UpdateGame()
        {
            //Update players.
            Harmony.Update(World.CurrentTime);
            Dischord.Update(World.CurrentTime);
            Jousting.Jouster.CollisionData colDat = Jousting.Jouster.CheckCollision(Harmony, Dischord);

            //Update blockers.
            UniqueBlockerSprites.Clear();
            for (int i = 0; i < Blockers.Count; ++i)
            {
                Blockers[i].Update(World.CurrentTime);

                if (!UniqueBlockerSprites.Contains(Blockers[i].Sprite))
                    UniqueBlockerSprites.Add(Blockers[i].Sprite);
            }
            for (int i = 0; i < Blockers.Count; ++i)
            {
                for (int j = i + 1; j < Blockers.Count; ++j)
                {
                    Blocker.CheckCollision(Blockers[i], Blockers[j]);
                }
            }

            //Update animation.
            HarmonySprite.UpdateAnimation(World.CurrentTime);
            DischordSprite.UpdateAnimation(World.CurrentTime);

            //Update minigame logic.
            Update(colDat);
        }
        public void Draw(SpriteBatch sb)
        {
            DrawBelowPlayers(sb);

            float scale = WorldData.ZoomScaleAmount[CurrentZoom],
                  invScale = 1.0f / scale;

            HarmonySprite.DrawArgs.Rotation = Harmony.Rotation;
            HarmonySprite.DrawArgs.Scale *= scale;
            DischordSprite.DrawArgs.Rotation = Dischord.Rotation;
            DischordSprite.DrawArgs.Scale *= scale;

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);

            HarmonySprite.Draw(Harmony.Pos, sb);
            DischordSprite.Draw(Dischord.Pos, sb);
            for (int i = 0; i < Blockers.Count; ++i)
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


        protected abstract void Reset();
        protected abstract void Update(Jousting.Jouster.CollisionData playerCollision);

        protected virtual void DrawBelowPlayers(SpriteBatch sb) { }
        protected virtual void DrawAbovePlayers(SpriteBatch sb) { }
    }
}
