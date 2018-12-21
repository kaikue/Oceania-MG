using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.States
{
	public abstract class GameState
	{
		public abstract void Update(Input input, GameTime gameTime);

		public abstract void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime);
	}
}
