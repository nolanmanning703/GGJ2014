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
        private static bool WithinError(float f1, float f2, float error)
        {
            return (Math.Abs(f1 - f2) <= error);
        }


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
        public float GetCameraZoom(ZoomLevels zoom) { return 1.0f / WorldData.ZoomScaleAmount[zoom]; }
        public Matrix CamTransform { get; private set; }
        public Matrix CamTransformInverse { get; private set; }
        public bool ZoomingIn, ZoomingOut;

        //Zooming.
        private float zoomTarget = Single.NaN,
                      zoomStart = Single.NaN;
        private float zoomStartTime = Single.NaN;
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

                if (ZoomingIn) zoomTarget = GetCameraZoom(currentZoom);
                else zoomTarget = GetCameraZoom(currentZoom);
                zoomStart = Camera.Zoom;
                zoomStartTime = (float)CurrentTime.TotalGameTime.TotalSeconds;

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

            currentZoom = ZoomLevels.Five;

            Camera = new KarmaCamera(device);
            Camera.Zoom = GetCameraZoom(currentZoom);

            CurrentZoom = currentZoom;
            ZoomingIn = false;
            ZoomingOut = false;
            foreach (ZoomLevels zooms in WorldData.AscendingZooms)
                if (WorldData.Minigames[zooms] != null)
                    WorldData.Minigames[zooms].ResetGame();

            JoustingInput.InitializeInput();

            Camera.Update(new GameTime(TimeSpan.FromSeconds(0.016667), TimeSpan.FromSeconds(0.016667)));
            Camera.Zoom = GetCameraZoom(currentZoom);
        }

        public void Update(GameTime gt)
        {
            CurrentTime = gt;

            if (KS.IsKeyDown(Keys.G)) Special = 1.0f;


            Timers.Update(gt);


            if (ZoomingIn || ZoomingOut)
            {
                if (WithinError(Camera.Zoom, zoomTarget,
                                (ZoomingIn ? 0.01f / WorldData.ZoomScaleAmount[currentZoom] :
                                             0.01f / WorldData.ZoomScaleAmount[currentZoom])))
                {
                    Camera.Zoom = zoomTarget;
                    Camera.NonShakePos = Vector2.Zero;
                    Camera.Pos = Camera.NonShakePos;
                    ZoomingIn = false;
                    ZoomingOut = false;
                }
            }

            float deltaT = (float)gt.TotalGameTime.TotalSeconds - zoomStartTime;
            if (ZoomingIn)
            {
                float progress = WorldData.GetCurrentZoom(Camera.Zoom).LinearInterpolant;
                if (progress < 0.0f || progress > 1.0f)
                {
                    Console.WriteLine(progress);
                }

                //Change position.
                if (CurrentZoom == WorldData.ZoomIn(ZoomLevels.Five))
                {
                    const double pow = 50.0;
                    Camera.NonShakePos = Vector2.Lerp(WorldData.Minigames[ZoomLevels.Five].Harmony.Pos, Vector2.Zero, (float)Math.Pow(1.0f - progress, pow));
                    Camera.Pos = Camera.NonShakePos;
                }

                //Change zoom.
                Camera.Zoom = MathHelper.Lerp(Camera.Zoom, zoomTarget, deltaT * 0.025f);
            }
            else if (ZoomingOut)
            {
                float progress = WorldData.GetCurrentZoom(Camera.Zoom).LinearInterpolant;
                if (progress < 0.0f || progress > 1.0f)
                {
                    Console.WriteLine(progress);
                }

                //Change position.
                if (CurrentZoom == ZoomLevels.Five)
                {

                    const double pow = 30.0;
                    Camera.NonShakePos = Vector2.Lerp(WorldData.Minigames[ZoomLevels.Five].Harmony.Pos, Vector2.Zero, (float)Math.Pow(1.0f - progress, pow));
                    Camera.Pos = Camera.NonShakePos;
                }

                //Change zoom.
                Camera.Zoom = MathHelper.Lerp(Camera.Zoom, zoomTarget, deltaT * 0.025f);
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
                if (Jousting.JoustingInput.IsPressingHoldSpecial(Jousters.Harmony) &&
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

                    Color col = Color.White;
                    sb.Draw(ArtAssets.WorldBackgrounds[zoom], Vector2.Zero, null, col, 0.0f,
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
                Camera.Pos = Camera.NonShakePos;

                CamTransform = Camera.TransformMatrix;
                CamTransformInverse = Matrix.Invert(CamTransform);

            }
            RenderWorldToTexture(gt, sb);
            if (seeingFive)
            {
                Camera.Zoom = oldZoom;
                Camera.NonShakePos = pos;
                Camera.Pos = Camera.NonShakePos;

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

            ArtAssets.DrawSpecialBar(sb, Special, new Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight));

            sb.End();
        }
    }
}
