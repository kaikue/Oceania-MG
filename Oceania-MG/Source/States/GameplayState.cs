using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Oceania_MG.Source.States
{
	class GameplayState : GameState
	{
		public const int BLOCK_SIZE = 16;
		public const float SCALE = 2; //TODO: make this a variable option
		public const int SCALED_BLOCK_SIZE = (int)(BLOCK_SIZE * SCALE);

		private World world;

		private Vector2 viewport;

		public GameplayState()
		{
			world = new World("defaultworld", 100); //TODO
			viewport = new Vector2();
			UpdateViewport();
		}

		private void UpdateViewport()
		{
			Point playerCenter = world.GetPlayer().GetCenter();
			viewport.X = playerCenter.X - Game.GetWidth() / (2 * SCALE);
			viewport.Y = playerCenter.Y - Game.GetHeight() / (2 * SCALE);
		}

		public Vector2 GetViewport()
		{
			return viewport;
		}

		public override void Update(Input input, GameTime gameTime)
		{
			world.Update(input, gameTime);
			UpdateViewport();
		}

		public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			graphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			world.Draw(graphicsDevice, spriteBatch, gameTime);

			float fps = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
			Vector2 playerPos = world.GetPlayer().GetPosition();
			string biome = world.GetBiomeAt((int)playerPos.X, (int)playerPos.Y).name;
			string info = "FPS: " + fps + "\n[" + playerPos.X + ", " + playerPos.Y + "]\nBiome: " + biome;
			spriteBatch.DrawString(Game.GetFont(), info, new Vector2(50, 50), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);

			spriteBatch.End();
		}

	}
}
