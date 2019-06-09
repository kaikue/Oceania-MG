using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class Button : GUIElement
	{
		private Color bodyColor = Color.White;
		private Color outlineColor = Color.Black;
		private Color hoveredColor = Color.LightGray;
		private Color pressedColor = Color.DimGray;
		private Color draggedColor = Color.LightGray;
		private Color labelColor = Color.Black;

		private string label;
		private Action clickAction;

		private Rectangle bodyRect;
		private Vector2 labelPos;

		private enum ButtonState
		{
			None,
			Hovered,
			Pressed,
			DraggedAway
		}

		private ButtonState state = ButtonState.None;

		public Button(Rectangle bounds, string label, Action clickAction) : base(bounds)
		{
			this.label = label;
			this.clickAction = clickAction;

			RefreshBounds();
		}

		protected override void RefreshBounds()
		{
			Point outlineOffset = new Point(scale, scale); //scaled-thickness outline
			bodyRect = new Rectangle(bounds.Location + outlineOffset, bounds.Size - outlineOffset - outlineOffset);

			Vector2 labelSize = font.MeasureString(label) * scale;
			labelPos = new Vector2(bounds.Center.X - labelSize.X / 2, bounds.Center.Y - labelSize.Y / 2);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(pixel, bounds, outlineColor);

			Color col = bodyColor;
			if (state == ButtonState.Hovered) col = hoveredColor;
			else if (state == ButtonState.Pressed) col = pressedColor;
			else if (state == ButtonState.DraggedAway) col = draggedColor;

			spriteBatch.Draw(pixel, bodyRect, col);
			
			spriteBatch.DrawString(font, label, labelPos, labelColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public override void Update(Input input)
		{
			base.Update(input);

			if (hovered)
			{
				if (state == ButtonState.None) state = ButtonState.Hovered;
				else if (state == ButtonState.DraggedAway) state = ButtonState.Pressed;
			}
			else
			{
				if (state == ButtonState.Hovered) state = ButtonState.None;
				else if (state == ButtonState.Pressed) state = ButtonState.DraggedAway;
			}
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (hovered && control == Input.Controls.LeftClick)
			{
				state = ButtonState.Pressed;
			}
		}

		public override void ControlReleased(Input.Controls control)
		{
			if (control == Input.Controls.LeftClick)
			{
				//Only perform the action if this is the same button that was pressed initially, otherwise just release
				if (state == ButtonState.Pressed)
				{
					clickAction();
					state = ButtonState.Hovered;
				}
				else if (state == ButtonState.DraggedAway)
				{
					state = ButtonState.None;
				}
			}
		}
	}
}
