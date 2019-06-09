using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class SelectableImage : GUIElement
	{
		private Color outlineColor = Color.Black;
		
		private Action selectAction;

		private Rectangle outlineRect;
		private Texture2D texture;
		private bool selected = false;

		public SelectableImage(Rectangle bounds, Texture2D texture, Action selectAction) : base(bounds)
		{
			this.texture = texture;
			this.selectAction = selectAction;

			RefreshBounds();
		}

		protected override void RefreshBounds()
		{
			Point outlineOffset = new Point(scale, scale); //scaled-thickness outline around texture
			outlineRect = new Rectangle(bounds.Location - outlineOffset, bounds.Size + outlineOffset + outlineOffset);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (selected)
			{
				spriteBatch.Draw(pixel, outlineRect, outlineColor);
			}

			spriteBatch.Draw(texture, ConvertUtils.PointToVector2(bounds.Location), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (hovered && control == Input.Controls.LeftClick)
			{
				selected = true;
				selectAction();
			}
		}

		public void Deselect()
		{
			selected = false;
		}
	}
}
