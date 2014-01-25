using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gamejam2014
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KarmaWorld world;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Utilities.Graphics.TexturePrimitiveDrawer.MakeBlankDrawingTex(GraphicsDevice);
            WorldData.Initialize(GraphicsDevice);
            ArtAssets.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_1.ArtAssets1.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_2.ArtAssets2.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_3.ArtAssets3.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_4.ArtAssets4.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_5.ArtAssets5.Initialize(GraphicsDevice, Content);
            Minigames.Minigame_1.SoundAssets1.Initialize(Content);
            Minigames.Minigame_2.SoundAssets2.Initialize(Content);
            Minigames.Minigame_3.SoundAssets3.Initialize(Content);
            Minigames.Minigame_4.SoundAssets4.Initialize(Content);
            Minigames.Minigame_5.SoundAssets5.Initialize(Content);

            graphics.PreferredBackBufferWidth = ArtAssets.WorldBackgrounds[ZoomLevels.One].Width;
            graphics.PreferredBackBufferHeight = ArtAssets.WorldBackgrounds[ZoomLevels.One].Height;
            graphics.ApplyChanges();

            world = new KarmaWorld(GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            world.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            world.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
