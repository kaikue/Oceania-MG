using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Oceania_MG.Source
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
		private Color[][][] colors;
		private Random random = new Random();
		private string hoverBiomeName = "";
		private World world;

		private int depth;
		private int minDepth = 0;
		private int maxDepth = 250;

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
			world = new World("biometest", 0);

			colors = new Color[maxDepth - minDepth][][];
			for (int d = 0; d < maxDepth - minDepth; d++)
			{
				colors[d] = new Color[WIDTH][];

				for (int x = 0; x < WIDTH; x++)
				{
					colors[d][x] = new Color[HEIGHT];
					for (int y = 0; y < HEIGHT; y++)
					{
						float sX = (float)((2 * x) - WIDTH) / WIDTH;
						float sY = (float)((2 * y) - HEIGHT) / HEIGHT;
						Biome biome = world.GetBiome(sX, sY, d + minDepth);
						int[] c = biome.color;
						Color color = new Color(c[0], c[1], c[2]);
						colors[d][x][y] = color;
					}
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
			{
				Exit();
			}
			
			float mouseX = (float)(Mouse.GetState().X / SCALE * 2 - WIDTH) / WIDTH;
			float mouseY = (float)(Mouse.GetState().Y / SCALE * 2 - HEIGHT) / HEIGHT;
			Biome hoverBiome = world.GetBiome(mouseX, mouseY, depth);
			hoverBiomeName = mouseX + ", " + mouseY + ": " + hoverBiome.name;
			
			if (!Keyboard.GetState().IsKeyDown(Keys.Space))
			{
				depth++;
				if (depth >= maxDepth)
				{
					depth = minDepth;
				}
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
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					Color color = colors[depth - minDepth][x][y];
					spriteBatch.Draw(pixel, new Rectangle(x * SCALE, y * SCALE, SCALE, SCALE), color);
				}
			}

			spriteBatch.DrawString(font, "Depth: " + depth, new Vector2(10, 380), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.DrawString(font, hoverBiomeName, new Vector2(10, 10), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.End();

			base.Draw(gameTime);
        }
    }
}
