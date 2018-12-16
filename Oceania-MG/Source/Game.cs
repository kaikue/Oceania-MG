using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Oceania_MG
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
		private static Game instance;

		private const float SCALE = 3;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private SpriteFont font;
		private Texture2D image;
        
        public Game()
        {
			instance = this;
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
            // TODO: Add your initialization logic here

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

			image = Content.Load<Texture2D>("Images/player/body/peach/idle");
			font = Content.Load<SpriteFont>("Font/CodersCrux");
			// TODO: use this.Content to load your game content here
		}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
			spriteBatch.Dispose();
			base.UnloadContent();
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			spriteBatch.DrawString(font, "Hello Oceania", new Vector2(100, 100), Color.Black, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.Draw(image, new Vector2(400, 240), null, Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);

			spriteBatch.End();

			base.Draw(gameTime);
        }

		public static Texture2D LoadImage(string imageURL)
		{
			return instance.Content.Load<Texture2D>(imageURL);
		}
    }
}
