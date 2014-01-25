using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Utilities.Input;
using Utilities.Input.Buttons;

namespace gamejam2014
{
    /// <summary>
    /// The game world.
    /// </summary>
    public class KarmaWorld
    {
        //Keep a static reference to this world to simplify matters. After all, there's only ever one instance at a time.
        public static KarmaWorld World = null;

        public GraphicsDevice GraphicsDevice;
        public ContentManager ContentManager;

        public KeyboardState KS;
        public MouseState MS;
        public GamePadState GPSOne;

        public ButtonInputManager Input;
        public Utilities.TimerManager Timers;

        public GameTime CurrentTime;

        public KarmaCamera Camera;

        private ZoomLevels currentZoom;
        public ZoomLevels CurrentZoom
        {
            get
            {
                return currentZoom;
            }

            set
            {
                currentZoom = value;

                Camera.ZoomData.MaxZoomSpeed = WorldData.CameraZoomSpeed(value);

                CurrentMinigame = WorldData.Minigames[value];
                if (CurrentMinigame != null) CurrentMinigame.ResetGame();

                SoundAssets.SwitchZoomMusic(value);
            }
        }

        public Minigame CurrentMinigame = null;


        public KarmaWorld(GraphicsDevice device, ContentManager content)
        {
            KarmaWorld.World = this;

            Camera = new KarmaCamera(device);
            Camera.ZoomData.ZoomGivenDist = d => WorldData.ZoomScaleAmount[CurrentZoom];
            Camera.Zoom = WorldData.ZoomScaleAmount[CurrentZoom];

            GraphicsDevice = device;
            ContentManager = content;

            CurrentZoom = ZoomLevels.Three;

            Timers = new Utilities.TimerManager();
            Input = new ButtonInputManager();

            Input.AddInput("Zoom In", new KeyboardButton(Keys.Up, true));
            Input.AddInput("Zoom Out", new KeyboardButton(Keys.Down, true));

            CurrentTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

            CurrentMinigame = WorldData.Minigames[CurrentZoom];
            if (CurrentMinigame != null) CurrentMinigame.ResetGame();
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
                CurrentMinigame.Update();
                if (CurrentMinigame.MoveDown)
                {
                    CurrentZoom = WorldData.ZoomIn(CurrentZoom);
                }
                else if (CurrentMinigame.MoveUp)
                {
                    CurrentZoom = WorldData.ZoomOut(CurrentZoom);
                }
            }
        }

        public void Draw(GameTime gt, SpriteBatch sb)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sb.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null, Camera.TransformMatrix);

            foreach (ZoomLevels zoom in WorldData.DescendingZooms)
            {
                sb.Draw(ArtAssets.WorldBackgrounds[zoom], Vector2.Zero, null, Color.White, 0.0f,
                        ArtAssets.GetBackgroundOrigin(zoom), WorldData.ZoomScaleAmount[zoom], SpriteEffects.None,
                        ArtAssets.WorldBackgroundLayerMaxes[zoom]);
            }
            foreach (ZoomLevels zoom in WorldData.DescendingZooms)
            {
                if (WorldData.Minigames[zoom] != null) WorldData.Minigames[zoom].Draw(sb);
            }
            CurrentMinigame.Draw(sb);

            sb.End();
        }
    }
}
