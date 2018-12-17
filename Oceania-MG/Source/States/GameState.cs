using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.States
{
	abstract class GameState
	{
		//TODO: handle input

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(GameTime gameTime);
	}
}
