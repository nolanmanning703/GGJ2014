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

        //Render target (needed for zoom level five).
        private RenderTarget2D RenderedWorld;
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
        public bool ZoomingIn = false, ZoomingOut = false;

        //Zooming.
        private float zoomTarget = Single.NaN,
                      zoomStart = Single.NaN;
        private ZoomLevels currentZoom;
        public ZoomLevels CurrentZoom
        {
            get
            {
                return currentZoom;
            }

            set
            {
                ZoomingIn = (value == WorldData.ZoomIn(currentZoom));
                ZoomingOut = !ZoomingIn;

                if (ZoomingOut && CurrentZoom == ZoomLevels.Four)
                {
                    Camera.Pos = WorldData.Minigames[ZoomLevels.Five].Harmony.Pos;
                }

                currentZoom = value;

                CurrentMinigame = WorldData.Minigames[value];
                if (CurrentMinigame != null) CurrentMinigame.ResetGame();

                if (!ZoomingIn) Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomInverse(CurrentZoom)];
                else         Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomIn(WorldData.ZoomInverse(CurrentZoom))];
                Camera.ZoomData.MaxZoomSpeed *= WorldData.CameraZoomSpeedScale;

                zoomTarget = Camera.ZoomData.ZoomGivenDist(0.0f);
                zoomStart = Camera.Zoom;

                //if ((ZoomingIn && value == WorldData.ZoomIn(ZoomLevels.Five)))
                //{
                //    Camera.K_CamTarget = WorldData.Minigames[ZoomLevels.Five].Harmony.Pos;
                //    Camera.K_Position = WorldData.Minigames[ZoomLevels.Five].Harmony.Pos;
                //}
                if (ZoomingOut && value == ZoomLevels.Five)
                {
                    Camera.NonShakePos = WorldData.Minigames[ZoomLevels.Five].Harmony.Pos;
                    Camera.Pos = Camera.NonShakePos;
                }

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

            Camera.K_CamTarget = Camera.NonShakePos;
            Camera.K_Position = Camera.NonShakePos;

            //Camera.Update(gt);
            Timers.Update(gt);

            if (ZoomingIn)
            {
                Camera.ZoomData.MaxZoomSpeed *= 1.01f;

                if (CurrentZoom == WorldData.ZoomIn(ZoomLevels.Five))
                {
                    float progress = WorldData.GetCurrentZoom(Camera.Zoom).LinearInterpolant;

                    //Change position.
                    const double pow = 0.1;
                    Camera.NonShakePos = Vector2.Lerp(Vector2.Zero, WorldData.Minigames[ZoomLevels.Five].Harmony.Pos, (float)Math.Pow(1.0f - progress, pow));

                    //Change zoom.
                    const double pow2 = 0.1;
                    Camera.Zoom = MathHelper.Lerp(Camera.Zoom, zoomTarget, 0.01f);
                }
            }
            else if (ZoomingOut)
            {
                Camera.ZoomData.MaxZoomSpeed *= 0.999f;

                if (CurrentZoom == ZoomLevels.Five)
                {
                    float progress = WorldData.GetCurrentZoom(Camera.Zoom).LinearInterpolant;

                    //Change position.
                    const double pow = 10.0;
                    Camera.NonShakePos = Vector2.Lerp(WorldData.Minigames[ZoomLevels.Five].Harmony.Pos, Vector2.Zero, (float)Math.Pow(progress, pow));

                    //Change zoom.
                    const double pow2 = 0.1;
                    Camera.Zoom = MathHelper.Lerp(Camera.Zoom, zoomTarget, 0.01f);
                }
            }

            if ((ZoomingIn || ZoomingOut) && Camera.Zoom == Camera.TargetZoom)
            {
                Camera.NonShakePos = Vector2.Zero;
                ZoomingIn = false;
                ZoomingOut = false;
            }

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
            

            if (!ZoomingOut && CurrentMinigame != null)
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

        private void RenderWorldToTexture(GameTime gt, SpriteBatch sb)
        {
            GraphicsDevice.SetRenderTarget(RenderedWorld);

            GraphicsDevice.Clear(Color.Black);

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

            GraphicsDevice.SetRenderTarget(null);
            RenderedWorldTex = (Texture2D)RenderedWorld;
        }
        public void Draw(GameTime gt, SpriteBatch sb)
        {
            bool seeingFive = currentZoom == ZoomLevels.Five ||
                              (CurrentZoom == WorldData.ZoomIn(ZoomLevels.Five) && ZoomingIn);

            //Render up to zoom level 4.
            float oldZoom = Camera.Zoom;
            Vector2 pos = Camera.NonShakePos;
            if (seeingFive)
            {
                Camera.Zoom = 1.0f / WorldData.ZoomScaleAmount[ZoomLevels.Three];
                Camera.NonShakePos = Vector2.Zero;
                Camera.Pos = Vector2.Zero;

                CamTransform = Camera.TransformMatrix;
                CamTransformInverse = Matrix.Invert(CamTransform);

            }
            RenderWorldToTexture(gt, sb);
            if (seeingFive)
            {
                Camera.Zoom = oldZoom;
                Camera.NonShakePos = pos;
                Camera.Pos = pos;

                CamTransform = Camera.TransformMatrix;
                CamTransformInverse = Matrix.Invert(CamTransform);
            }
            
            GraphicsDevice.Clear(Color.Black);

            if (seeingFive)
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, CamTransform);
                sb.Draw(ArtAssets.WorldBackgrounds[ZoomLevels.Five], Vector2.Zero, null, Color.White, 0.0f,
                        ArtAssets.GetBackgroundOrigin(ZoomLevels.Five), WorldData.ZoomScaleAmount[ZoomLevels.Five], SpriteEffects.None,
                        ArtAssets.WorldBackgroundLayerMaxes[ZoomLevels.Five]);
                sb.End();
                if (WorldData.Minigames[ZoomLevels.Five] != null) WorldData.Minigames[ZoomLevels.Five].Draw(sb);
            }
            else
            {
                sb.Begin();
                sb.Draw(RenderedWorldTex, new Rectangle(0, 0, RenderedWorldTex.Width, RenderedWorldTex.Height), Color.White);
                sb.End();
            }


            //Global HUD.

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            sb.DrawString(ArtAssets.DebugFont, WorldData.GetCurrentZoom(Camera.Zoom).ToString(), Vector2.Zero, Color.White);
            ArtAssets.DrawSpecialBar(sb, Special, new Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight));

            sb.End();
        }
    }
}
