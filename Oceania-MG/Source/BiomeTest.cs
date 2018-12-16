using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oceania_MG.Source;
using System;

namespace Oceania_MG
{
    /// <summary>
    /// Shows world generator
    /// </summary>
    public class BiomeTest : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private SpriteFont font;
		private Texture2D pixel;
		private Color[][] colors;
		private Random random = new Random();
		private string hoverBiomeName = "";
		private World world;
		
		private const int WIDTH = 200;
		private const int HEIGHT = 200;
		private const int SCALE = 2;

        public BiomeTest()
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
			world = new World("BiomeTest", seed);
			for (int x = 0; x < WIDTH; x++)
			{
				colors[x] = new Color[HEIGHT];
				for (int y = 0; y < HEIGHT; y++)
				{
					//Tuple<float, float> values = gen.Terrain(x, y, 50, 150);
					float sX = (float)((2 * x) - WIDTH) / WIDTH;
					float sY = (float)((2 * y) - HEIGHT) / HEIGHT;
					Biome biome = world.GetBiome(sX, sY);
					int[] c = biome.color;
					Color color = new Color(c[0], c[1], c[2]);
					colors[x][y] = color;
				}
			}
		}

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font/CodersCrux");

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

			if (Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				Generate();
			}

			float mouseX = (float)(Mouse.GetState().X / SCALE * 2 - WIDTH) / WIDTH;
			float mouseY = (float)(Mouse.GetState().Y / SCALE * 2 - HEIGHT) / HEIGHT;
			Biome hoverBiome = world.GetBiome(mouseX, mouseY);
			hoverBiomeName = mouseX + ", " + mouseY + ": " + hoverBiome.name;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					Color color = colors[x][y];
					spriteBatch.Draw(pixel, new Rectangle(x * SCALE, y * SCALE, SCALE, SCALE), color);
				}
			}

			spriteBatch.DrawString(font, hoverBiomeName, new Vector2(10, 10), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.End();

			base.Draw(gameTime);
        }
    }
}
