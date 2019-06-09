using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Oceania_MG.Source
{
    /// <summary>
    /// Shows world generator
    /// </summary>
    public class GenerateTest : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private SpriteFont font;
		private Texture2D pixel;
		private Color[][] colors;
		private Random random = new Random();
		private World world;
		private Resources resources;
		private string hoverBiomeName;

		private const int WIDTH = 200;
		private const int START_Y = -100;
		private const int END_Y = 500;
		private const int HEIGHT = END_Y - START_Y;
		private const int SCALE = 1;

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
		
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font/CodersCrux");

			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });

			resources = new Resources(this);
			resources.LoadAll();
		}

		private void Generate()
		{
			colors = new Color[WIDTH][];

			int seed = random.Next();
			world = new World("BiomeTest", seed, resources);

			for (int x = 0; x < WIDTH; x++)
			{
				colors[x] = new Color[HEIGHT];
				for (int y = START_Y; y < END_Y; y++)
				{
					Biome biome = world.GetBiomeAt(x, y);
					Tuple<float, float> values = world.generate.Terrain(x, y, biome.minHeight, biome.maxHeight);
					float value = values.Item2;
					value = value < -0.5f ? (values.Item1 < -0.4f ? 0.1f : 0.25f) : value < -0.4f ? 0.8f : 1;
					if (y < World.SEA_LEVEL) value /= 2;

					int[] c = biome.color;
					Color color = new Color((int)(c[0] * value), (int)(c[1] * value), (int)(c[2] * value));

					if (values.Item2 > -0.5)
					{
						foreach (string oreName in biome.ores)
						{
							Ore ore = resources.GetOre(oreName);
							if (world.generate.Ore(x, y, oreName, ore.scale) > ore.cutoff)
							{
								color = new Color((uint)oreName.GetHashCode());
							}
						}
					}

					colors[x][y - START_Y] = color;
				}
			}
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

			int mouseX = Mouse.GetState().X / SCALE;
			int mouseY = Mouse.GetState().Y / SCALE + START_Y;
			Biome hoverBiome = world.GetBiomeAt(mouseX, mouseY);
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
