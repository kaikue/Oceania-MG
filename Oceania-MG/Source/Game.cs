using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oceania_MG.Source.States;
using System;

namespace Oceania_MG.Source
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
		private static Game instance;

		private const int WINDOWED_WIDTH = 800;
		private const int WINDOWED_HEIGHT = 600;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private GameState state;
		private Input input;

		private SpriteFont font;
		private Texture2D pixelTexture;

		private bool isDebugMode = true;
        
        public Game()
        {
			instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			//TODO: remember fullscreen setting
			graphics.PreferredBackBufferWidth = WINDOWED_WIDTH;
			graphics.PreferredBackBufferHeight = WINDOWED_HEIGHT;
			graphics.ApplyChanges();
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
			IsMouseVisible = true;
			input = new Input();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			state = new GameplayState();

			font = Content.Load<SpriteFont>("Font/CodersCrux");

			pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
			pixelTexture.SetData(new Color[] { Color.Black });
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
			base.UnloadContent();
			spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			input.Update();
			
			state.Update(input, gameTime);

			if (input.ControlPressed(Input.Controls.Fullscreen))
			{
				ToggleFullscreen();
			}
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			state.Draw(GraphicsDevice, spriteBatch, gameTime);

			base.Draw(gameTime);
        }

		private void ToggleFullscreen()
		{
			if (graphics.IsFullScreen)
			{
				graphics.PreferredBackBufferWidth = WINDOWED_WIDTH;
				graphics.PreferredBackBufferHeight = WINDOWED_HEIGHT;
				graphics.IsFullScreen = false;
				graphics.ApplyChanges();
			}
			else
			{
				graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
				graphics.IsFullScreen = true;
				graphics.ApplyChanges();
			}
		}
		
		public static Texture2D LoadImage(string imageURL)
		{
			if (instance == null) return null; //For GenerateTest, which needs World but not Game

			if (string.IsNullOrEmpty(imageURL))
			{
				return new Texture2D(instance.GraphicsDevice, GameplayState.BLOCK_SIZE, GameplayState.BLOCK_SIZE);
			}
			return instance.Content.Load<Texture2D>(imageURL);
		}

		public static SpriteFont GetFont()
		{
			return instance.font;
		}

		public static void SetState(GameState newState)
		{
			instance.state = newState;
		}

		public static int GetWidth()
		{
			return instance.graphics.PreferredBackBufferWidth;
		}

		public static int GetHeight()
		{
			return instance.graphics.PreferredBackBufferHeight;
		}

		public static Vector2 GetViewport()
		{
			if (instance.state is GameplayState)
			{
				return ((GameplayState)instance.state).GetViewport();
			}
			throw new InvalidOperationException("Game not in Gameplay state");
		}

		public static bool IsDebugMode()
		{
			return instance.isDebugMode;
		}

		public static Texture2D GetPixelTexture()
		{
			return instance.pixelTexture;
		}
	}
}
