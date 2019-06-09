using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class Panel : GUIElement
	{
		private Color bodyColor = Color.White;
		private Color outlineColor = Color.Black;

		private Rectangle bodyRect;

		public Panel(Rectangle bounds) : base(bounds)
		{
			Point outlineOffset = new Point(scale, scale); //scaled-thickness outline
			bodyRect = new Rectangle(bounds.Location + outlineOffset, bounds.Size - outlineOffset - outlineOffset);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(pixel, bounds, outlineColor);
			spriteBatch.Draw(pixel, bodyRect, bodyColor);
		}
	}
}
