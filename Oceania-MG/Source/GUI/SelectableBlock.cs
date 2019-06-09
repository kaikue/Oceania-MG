using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class SelectableBlock : GUIElement
	{
		private Color outlineColor = Color.Black;
		private Color backgroundColor = Color.White;
		
		private Action selectAction;

		private Rectangle outlineRect;
		private Block block;
		private bool selected = false;

		public SelectableBlock(Rectangle bounds, Block block) : base(bounds)
		{
			this.block = block;

			RefreshBounds();
		}

		public void SetSelectAction(Action selectAction)
		{
			this.selectAction = selectAction;
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
				spriteBatch.Draw(pixel, bounds, backgroundColor);
			}

			block.DrawSimple(spriteBatch, ConvertUtils.PointToVector2(bounds.Location), scale);
			//spriteBatch.Draw(texture, ConvertUtils.PointToVector2(bounds.Location), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
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

		public Block GetBlock()
		{
			return block;
		}
	}
}
