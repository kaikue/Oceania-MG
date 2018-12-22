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
		private const float SCALE = 3;

		private World world;

		private string ctrlString = "";

		public GameplayState()
		{
			world = new World("defaultworld", 100); //TODO
		}

		public override void Update(Input input, GameTime gameTime)
		{
			if (input.ControlHeld(Input.Controls.HotbarPrev))
			{
				ctrlString = "HotbarPrev";
			}
			else
			{
				ctrlString = "Not HotbarPrev";
			}
		}

		public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			graphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			Texture2D texture = world.GetBlock("basaltBricks").texture;
			for (int x = 0; x < 25; x++)
			{
				for (int y = 0; y < 20; y++)
				{
					spriteBatch.Draw(texture, new Vector2(x * Game.BLOCK_SIZE * SCALE, y * Game.BLOCK_SIZE * SCALE), null, Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
				}
			}

			float fps = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
			spriteBatch.DrawString(Game.GetFont(), "FPS: " + fps, new Vector2(100, 100), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			//spriteBatch.DrawString(Game.GetFont(), ctrlString, new Vector2(100, 200), Color.Black, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);

			spriteBatch.End();
		}

	}
}
