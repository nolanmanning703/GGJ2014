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

                currentZoom = value;

                if (!zoomIn) Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomInverse(CurrentZoom)];
                else Camera.ZoomData.MaxZoomSpeed = WorldData.ZoomScaleAmount[WorldData.ZoomIn(WorldData.ZoomInverse(CurrentZoom))];
                Camera.ZoomData.MaxZoomSpeed *= 10.0f;
                //if (!zoomIn) Camera.ZoomData.MaxZoomSpeed = WorldData.CameraZoomSpeed(value);
                //else Camera.ZoomData.MaxZoomSpeed = WorldData.CameraZoomSpeed(WorldData.ZoomIn(value));

                CurrentMinigame = WorldData.Minigames[value];
                if (CurrentMinigame != null) CurrentMinigame.ResetGame();

                SoundAssets.SwitchZoomMusic(value);
            }
        }

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

            Timers = new Utilities.TimerManager();
            Input = new ButtonInputManager();

            Input.AddInput("Zoom In", new KeyboardButton(Keys.Up, true));
            Input.AddInput("Zoom Out", new KeyboardButton(Keys.Down, true));

            CurrentTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

            currentZoom = ZoomLevels.One;

            Camera = new KarmaCamera(device);
            Camera.ZoomData.ZoomGivenDist = d => 1.0f / WorldData.ZoomScaleAmount[CurrentZoom];
            Camera.Zoom = WorldData.ZoomScaleAmount[currentZoom];

            CurrentZoom = ZoomLevels.One;//Three;
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

            if (Input.GetBoolInput("Zoom In").Value) CurrentZoom = WorldData.ZoomIn(CurrentZoom);
            if (Input.GetBoolInput("Zoom Out").Value) CurrentZoom = WorldData.ZoomOut(CurrentZoom);


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


            //Backgrounds.

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, CamTransform);

            foreach (ZoomLevels zoom in WorldData.DescendingZooms)
            {
                sb.Draw(ArtAssets.WorldBackgrounds[zoom], Vector2.Zero, null, Color.White, 0.0f,
                        ArtAssets.GetBackgroundOrigin(zoom), WorldData.ZoomScaleAmount[zoom], SpriteEffects.None,
                        ArtAssets.WorldBackgroundLayerMaxes[zoom]);
            }

            sb.End();


            //Minigames.
            foreach (ZoomLevels zoom in WorldData.DescendingZooms)
            {
                if (WorldData.Minigames[zoom] != null)
                    WorldData.Minigames[zoom].Draw(sb);
            }


            //Global HUD.

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            sb.DrawString(ArtAssets.DebugFont, WorldData.GetCurrentZoom(Camera.Zoom).ToString(), Vector2.Zero, Color.White);
            sb.DrawString(ArtAssets.DebugFont, CurrentZoom.ToString() + ", " + Camera.ZoomData.MaxZoomSpeed, new Vector2(50.0f), Color.White);
            
            Vector2 mousePos = new Vector2(MS.X, MS.Y);
            sb.DrawString(ArtAssets.DebugFont, Vector2.Transform(mousePos, CamTransformInverse).ToString(), mousePos, Color.White);

            sb.End();
        }
    }
}
