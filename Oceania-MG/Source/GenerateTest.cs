using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Oceania_MG
{
    /// <summary>
    /// Shows world generator
    /// </summary>
    public class GenerateTest : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Texture2D pixel;
		private Color[][] colors;
		private Random random = new Random();
		
		private const int WIDTH = 200;
		private const int HEIGHT = World.HEIGHT;
		private const int SCALE = 2;

        public GenerateTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = WIDTH * SCALE;
			graphics.PreferredBackBufferHeight = HEIGHT * SCALE;
			graphics.ApplyChanges();
		}
		
        protected override void Initialize()
        {
			base.Initialize();
			Generate();
			IsMouseVisible = true;
		}
		
		private void Generate()
		{
			colors = new Color[WIDTH][];

			int seed = random.Next();
			Generate gen = new Generate(seed);
			for (int x = 0; x < WIDTH; x++)
			{
				colors[x] = new Color[HEIGHT];
				for (int y = 0; y < HEIGHT; y++)
				{
					Tuple<float, float> values = gen.Terrain(x, y, 50, 150);
					float value = values.Item1;
					int w = 128 + (int)(127 * value);
					Color color = new Color(w, w, w);
					if (value > -0.5)
					{
						color.R = 255;
					}
					else
					{
						color.B = 255;
					}
					colors[x][y] = color;
				}
			}
		}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
			spriteBatch.Dispose();
			pixel.Dispose();
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

			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				Generate();
			}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();

			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					Color color = colors[x][y];
					spriteBatch.Draw(pixel, new Rectangle(x * SCALE, y * SCALE, SCALE, SCALE), color);
				}
			}
			spriteBatch.End();

			base.Draw(gameTime);
        }
    }
}
