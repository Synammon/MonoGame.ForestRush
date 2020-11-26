using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Psilibrary;
using Psilibrary.StateManager;
using Psilibrary.TileEngine;

namespace ForestRush
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly StateManager stateManager;
        private readonly TextureManager textureManager;
        private readonly ScrollingBackgroundManager scrollingBackgroundManager;

        private SpriteBatch spriteBatch;

        private Camera camera;
        private Engine engine;
        private World world;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public Game1()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            stateManager = new StateManager(this);
            Components.Add(stateManager);

            textureManager = new TextureManager(this);
            Components.Add(textureManager);

            scrollingBackgroundManager = new ScrollingBackgroundManager(this);
            Components.Add(scrollingBackgroundManager);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

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

            // TODO: use this.Content to load your game content here
            world = new World();
            
            Tileset tileset = new Tileset(
                Content.Load<Texture2D>(@"Tileset"),
                8,
                6,
                16,
                16);

            MapLayer baseLayer = new MapLayer(new Tile[30, 100]);

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    baseLayer.SetTile(i, j, new Tile(-1, -1));
                }
            }

            for (int i = 0; i < 100; i++)
            {
                baseLayer.SetTile(i, 29, new Tile(1, 0));
            }

            TileMap map = new TileMap("test", tileset, baseLayer, new CollisionLayer(), new PortalLayer());
            map.PortalLayer.Portals.Add("test", new Portal(new Point(0, 0), new Point(0, 28), "test"));
            
            world.AddMap("test", map);
            world.ChangeMap("test", "test");
            
            camera = new Camera(new Rectangle(0, 0, 1280, 720));
            
            engine = new Engine(32, 32);

            scrollingBackgroundManager.AddBackground(
                "Sky",
                "BG1",
                Vector2.Zero,
                5.01f);
            scrollingBackgroundManager.AddBackground(
                "Mountains",
                "BG2",
                Vector2.Zero,
                10.25f);
            scrollingBackgroundManager.AddBackground(
                "Hills",
                "BG3",
                Vector2.Zero,
                20.5f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Update(gameTime, world.CurrentMap);
            camera.LockCamera(world.CurrentMap);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            scrollingBackgroundManager.Draw("Sky", SpriteBatch);
            scrollingBackgroundManager.Draw("Mountains", SpriteBatch);
            scrollingBackgroundManager.Draw("Hills", SpriteBatch);
            SpriteBatch.End();

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                camera.Transformation);

            world.CurrentMap.Draw(gameTime, spriteBatch, camera);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
