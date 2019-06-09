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
		private Color labelColor = Color.Black;

		private string label;

		private Rectangle bodyRect;

		public Panel(Rectangle bounds, string label = null) : base(bounds)
		{
			this.label = label;
			Point outlineOffset = new Point(scale, scale); //scaled-thickness outline
			bodyRect = new Rectangle(bounds.Location + outlineOffset, bounds.Size - outlineOffset - outlineOffset);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(pixel, bounds, outlineColor);
			spriteBatch.Draw(pixel, bodyRect, bodyColor);
			if (label != null)
			{
				Vector2 labelPos = new Vector2(bounds.X + 3 * scale, bounds.Y + 3 * scale);
				spriteBatch.DrawString(font, label, labelPos, labelColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			}
		}
	}
}
