using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Utilities.Input;
using Utilities.Input.Buttons;
using gamejam2014.Jousting;
using gamejam2014.Minigames;

namespace gamejam2014
{
    /// <summary>
    /// The game world.
    /// </summary>
    public class KarmaWorld
    {
        //Keep a static reference to this world to simplify matters.
        //After all, there's only ever one instance at a time.
        public static KarmaWorld World = null;

        //Graphical stuff.
        public GraphicsDevice GraphicsDevice;
        public ContentManager ContentManager;
        public RenderTarget2D RenderedWorld;
        public Texture2D RenderedWorldTex;

        //Input stuff.
        public KeyboardState KS;
        public MouseState MS;
        public GamePadState GPSOne;

        //System managers.
        public ButtonInputManager Input;
        public Utilities.TimerManager Timers;

        //Timing.
        public GameTime CurrentTime;

        //Camera.
        public KarmaCamera Camera;
        public Matrix CamTransform { get; private set; }
        public Matrix CamTransformInverse { get; private set; }
        private bool zoomingInFromFive = false;

        //Zooming.
        private ZoomLevels currentZoom;
        public ZoomLevels CurrentZoom
        {
            get
            {
                return currentZoom;
            }

            set
            {
                bool zoomIn = (value == WorldData.ZoomIn(currentZoom));
                if (zoomIn && CurrentZoom == ZoomLevels.Five)
                {
                    zoomingInFromFive = true;
                }

                currentZoom = value;

                if (!zoomIn) Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomInverse(CurrentZoom)];
                else         Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomIn(WorldData.ZoomInverse(CurrentZoom))];
                Camera.ZoomData.MaxZoomSpeed *= WorldData.CameraZoomSpeedScale;
                //if (!zoomIn) Camera.ZoomData.MaxZoomSpeed = WorldData.CameraZoomSpeed(value);
                //else Camera.ZoomData.MaxZoomSpeed = WorldData.CameraZoomSpeed(WorldData.ZoomIn(value));

                CurrentMinigame = WorldData.Minigames[value];
                if (CurrentMinigame != null) CurrentMinigame.ResetGame();

                SoundAssets.SwitchZoomMusic(value);
            }
        }

        //Specials. Range from 0 to 1.
        public float Special;
        public bool IsSpecialFull { get { return Special >= 1.0f; } }

        //Minigame-specific stuff.
        public JousterPhysicsData PhysicsData { get { return gamejam2014.PhysicsData.JoustingMinigamePhysics[CurrentZoom]; } }
        public Minigame CurrentMinigame = null;

        //World dimensions.
        public Vector2 WorldSize
        {
            get
            {
                return WorldData.ZoomScaleAmount[CurrentZoom] *
                       new Vector2(ArtAssets.WorldBackgrounds[CurrentZoom].Width,
                                   ArtAssets.WorldBackgrounds[CurrentZoom].Height);
            }
        }
        public Utilities.Math.Shape.Rectangle WorldBounds
        {
            get
            {
                Vector2 halfSize = 0.5f * WorldSize;
                return new Utilities.Math.Shape.Rectangle(-halfSize, halfSize);
            }
        }


        public KarmaWorld(GraphicsDevice device, ContentManager content)
        {
            KarmaWorld.World = this;

            GraphicsDevice = device;
            ContentManager = content;
            RenderedWorld = new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight,
                                               true, device.DisplayMode.Format, DepthFormat.Depth24);

            Timers = new Utilities.TimerManager();
            Input = new ButtonInputManager();

            Input.AddInput("Zoom In", new KeyboardButton(Keys.PageUp, true));
            Input.AddInput("Zoom Out", new KeyboardButton(Keys.PageDown, true));

            CurrentTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

            currentZoom = ZoomLevels.Five;//Three

            Camera = new KarmaCamera(device);
            Camera.ZoomData.ZoomGivenDist = d => 1.0f / WorldData.ZoomScaleAmount[CurrentZoom];
            Camera.Zoom = 1.0f / WorldData.ZoomScaleAmount[currentZoom];

            CurrentZoom = currentZoom;;
            foreach (ZoomLevels zooms in WorldData.AscendingZooms)
                if (WorldData.Minigames[zooms] != null)
                    WorldData.Minigames[zooms].ResetGame();

            JoustingInput.InitializeInput();
        }

        public void Update(GameTime gt)
        {
            CurrentTime = gt;


            Camera.Update(gt);
            Timers.Update(gt);


            KS = Keyboard.GetState();
            MS = Mouse.GetState();
            GPSOne = GamePad.GetState(PlayerIndex.One);
            Input.Update(gt, KS, MS, GPSOne);

            foreach (Utilities.Graphics.AnimatedSprite sprite in ArtAssets.SpecialUIAlert.Values)
                sprite.UpdateAnimation(gt);

            if (Input.GetBoolInput("Zoom In").Value) CurrentZoom = WorldData.ZoomIn(CurrentZoom);
            if (Input.GetBoolInput("Zoom Out").Value) CurrentZoom = WorldData.ZoomOut(CurrentZoom);

            if (Special == 1.0f)
            {
                if (Jousting.JoustingInput.IsPressingSpecial(Jousters.Harmony))
                {
                    CurrentMinigame.OnHarmonySpecial();
                    Special = 0.0f;
                }
                else if (Jousting.JoustingInput.IsPressingSpecial(Jousters.Dischord))
                {
                    CurrentMinigame.OnDischordSpecial();
                    Special = 0.0f;
                }
            }
            else
            {
                if (Jousting.JoustingInput.IsPressingHoldSpecial(Jousters.Harmony) ||
                    Jousting.JoustingInput.IsPressingHoldSpecial(Jousters.Dischord))
                {
                    if (Special < 1.0f)
                        Special += (float)gt.ElapsedGameTime.TotalSeconds * WorldData.SpecialGainPerSecond.Random();
                }
                if (Special > 1.0f)
                {
                    Special = 1.0f;
                    foreach (Utilities.Graphics.AnimatedSprite sprite in ArtAssets.SpecialUIAlert.Values)
                        sprite.ResetAnimation(gt);
                }
            }
            

            if (CurrentMinigame != null)
            {
                CurrentMinigame.UpdateGame();
                if (CurrentMinigame.MoveDown)
                {
                    CurrentZoom = WorldData.ZoomIn(CurrentZoom);
                }
                else if (CurrentMinigame.MoveUp)
                {
                    CurrentZoom = WorldData.ZoomOut(CurrentZoom);
                }
            }

            CamTransform = Camera.TransformMatrix;
            CamTransformInverse = Matrix.Invert(CamTransform);
        }

        public void Draw(GameTime gt, SpriteBatch sb)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            //Minigames.

            //Zoom level 5 shows the moving planet that the rest of the game takes place on. This requires a special rendering sequence.
            if (CurrentZoom != ZoomLevels.Five)
            {
                foreach (ZoomLevels zoom in WorldData.DescendingZooms)
                {
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, CamTransform);

                    sb.Draw(ArtAssets.WorldBackgrounds[zoom], Vector2.Zero, null, Color.White, 0.0f,
                            ArtAssets.GetBackgroundOrigin(zoom), WorldData.ZoomScaleAmount[zoom], SpriteEffects.None,
                            ArtAssets.WorldBackgroundLayerMaxes[zoom]);

                    sb.End();

                    if (WorldData.Minigames[zoom] != null)
                        WorldData.Minigames[zoom].Draw(sb);
                }
            }
            else
            {
                GraphicsDevice.SetRenderTarget(RenderedWorld);

                //Render the world to a render target.
                foreach (ZoomLevels zoom in WorldData.DescendingZooms)
                {
                    if (zoom != ZoomLevels.Five)
                    {
                        sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, CamTransform);

                        sb.Draw(ArtAssets.WorldBackgrounds[zoom], Vector2.Zero, null, Color.White, 0.0f,
                                ArtAssets.GetBackgroundOrigin(zoom), WorldData.ZoomScaleAmount[zoom], SpriteEffects.None,
                                ArtAssets.WorldBackgroundLayerMaxes[zoom]);

                        sb.End();

                        if (WorldData.Minigames[zoom] != null) WorldData.Minigames[zoom].Draw(sb);
                    }
                }


                //Now draw the fifth zoom level minigame (it automatically uses the rendered world texture).

                GraphicsDevice.SetRenderTarget(null);
                RenderedWorldTex = (Texture2D)RenderedWorld;

                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, CamTransform);
                sb.Draw(ArtAssets.WorldBackgrounds[ZoomLevels.Five], Vector2.Zero, null, Color.White, 0.0f,
                        ArtAssets.GetBackgroundOrigin(ZoomLevels.Five), WorldData.ZoomScaleAmount[ZoomLevels.Five], SpriteEffects.None,
                        ArtAssets.WorldBackgroundLayerMaxes[ZoomLevels.Five]);
                sb.End();
                if (WorldData.Minigames[ZoomLevels.Five] != null) WorldData.Minigames[ZoomLevels.Five].Draw(sb);
            }


            //Global HUD.

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            //sb.DrawString(ArtAssets.DebugFont, Special.ToString(), Vector2.Zero, Color.White);
            ArtAssets.DrawSpecialBar(sb, Special, new Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight));

            sb.End();
        }
    }
}
