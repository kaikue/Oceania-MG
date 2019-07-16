using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Oceania_MG.Source.GUI
{
	class TextBox : GUIElement
	{
		private Color bodyColor = Color.White;
		private Color outlineColor = Color.Black;
		private Color textColor = Color.Black;

		private enum TextBoxState
		{
			Deselected,
			CursorSelected,
			BlockSelected
		}

		public string text;
		private TextBoxState state = TextBoxState.Deselected;
		private int selectIndex = 0;
		private int blockSelectStart;
		private int blockSelectEnd;

		private Rectangle bodyRect;
		private Vector2 textPos;

		public TextBox(Rectangle bounds, GUIElement parent) : base(bounds, parent)
		{
			RefreshBounds();
		}

		protected override void RefreshBounds()
		{
			Point outlineOffset = new Point(scale, scale); //scaled-thickness outline
			bodyRect = new Rectangle(bounds.Location + outlineOffset, bounds.Size - outlineOffset - outlineOffset);

			Vector2 textSize = font.MeasureString("TEXT") * scale;
			textPos = new Vector2(bodyRect.X + scale, bounds.Center.Y - textSize.Y / 2);
		}

		public override void Update(Input input)
		{
			base.Update(input);

			//on mouse click & hover: start selecting, set selectIndex
			//on mouse hold: if current hovered (same x-position) character is not the same as the selectedIndex: blockselect
			//on mouse release: if was selecting: stop selecting
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(pixel, bounds, outlineColor);

			Color col = bodyColor;
			if (state == TextBoxState.CursorSelected)
			{
				//draw cursor line
			}
			else if (state == TextBoxState.BlockSelected)
			{
				//draw box over selection
			}

			spriteBatch.Draw(pixel, bodyRect, col);
			//TODO: crop to effective area
			//spriteBatch.Draw(pixel, bodyRect.Location, sourceRectangle, col);

			//TODO: scroll to the side if too long to fit
			spriteBatch.DrawString(font, text, textPos, textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}
	}
}
