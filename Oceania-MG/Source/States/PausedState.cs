using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Oceania_MG.Source.States
{
	class PausedState : MenuState
	{
		private GameplayState gameplayState;

		public PausedState(GameplayState gameplayState)
		{
			this.gameplayState = gameplayState;
		}

		public override void Update(Input input, GameTime gameTime)
		{
			//TODO
		}

		public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			//TODO
		}

	}
}
