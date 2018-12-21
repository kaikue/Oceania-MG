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
			//world = new World("defaultworld", 100); //TODO
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

			spriteBatch.DrawString(Game.GetFont(), "Hello Oceania", new Vector2(100, 100), Color.Black, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.DrawString(Game.GetFont(), ctrlString, new Vector2(100, 200), Color.Black, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			//spriteBatch.Draw(image, new Vector2(400, 240), null, Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);

			spriteBatch.End();
		}

	}
}
