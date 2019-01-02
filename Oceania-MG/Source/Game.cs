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

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private GameState state;
		private Input input;

		private SpriteFont font;
        
        public Game()
        {
			instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
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
			// TODO: use this.Content to load your game content here
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
	}
}
