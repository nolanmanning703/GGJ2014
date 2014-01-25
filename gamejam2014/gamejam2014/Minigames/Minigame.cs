using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

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

        public Jousting.Jouster Harmony, Dischord;
        public ZoomLevels CurrentZoom;

        protected Utilities.Graphics.AnimatedSprite HarmonySprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Harmony]; } }
        protected Utilities.Graphics.AnimatedSprite DischordSprite { get { return ArtAssets.PlayerSprites[CurrentZoom][Jousting.Jousters.Dischord]; } }


        public Minigame(ZoomLevels currentZoom) { CurrentZoom = currentZoom;  MoveUp = false; MoveDown = false; }

        public void ResetGame()
        {
            MoveUp = false;
            MoveDown = false;

            Harmony = new Jousting.Jouster(Jousting.Jousters.Harmony,
                                           WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Harmony));
            Dischord = new Jousting.Jouster(Jousting.Jousters.Dischord,
                                            WorldData.GetStartingPos(CurrentZoom, Jousting.Jousters.Dischord));

            Reset();
        }
        public void UpdateGame()
        {
            //Update players.
            Harmony.Update(World.CurrentTime);
            Dischord.Update(World.CurrentTime);
            Jousting.Jouster.CollisionData colDat = Jousting.Jouster.CheckCollision(Harmony, Dischord);

            //Update animation.
            HarmonySprite.UpdateAnimation(World.CurrentTime);
            DischordSprite.UpdateAnimation(World.CurrentTime);

            //Update minigame logic.
            Update(colDat);
        }
        public void Draw(SpriteBatch sb)
        {
            DrawBelowPlayers(sb);

            float scale = WorldData.ZoomScaleAmount[CurrentZoom];

            HarmonySprite.DrawArgs.Rotation = Harmony.Rotation;
            HarmonySprite.DrawArgs.Scale = new Microsoft.Xna.Framework.Vector2(scale, scale);
            DischordSprite.DrawArgs.Rotation = Dischord.Rotation;
            DischordSprite.DrawArgs.Scale = new Microsoft.Xna.Framework.Vector2(scale, scale);

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, World.CamTransform);
            HarmonySprite.Draw(Harmony.Pos, sb);
            DischordSprite.Draw(Dischord.Pos, sb);
            Microsoft.Xna.Framework.Color col = Microsoft.Xna.Framework.Color.White;
            col.A = 100;
            Utilities.Graphics.TexturePrimitiveDrawer.DrawShape(Harmony.ColShape, col, sb, 1);
            Utilities.Graphics.TexturePrimitiveDrawer.DrawShape(Dischord.ColShape, col, sb, 1);
            sb.End();

            DrawAbovePlayers(sb);
        }


        protected abstract void Reset();
        protected abstract void Update(Jousting.Jouster.CollisionData playerCollision);

        protected virtual void DrawBelowPlayers(SpriteBatch sb) { }
        protected virtual void DrawAbovePlayers(SpriteBatch sb) { }
    }
}
