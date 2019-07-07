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

		public SelectableBlock(Rectangle bounds, Block block, GUIElement parent) : base(bounds, parent)
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
			Rectangle effectiveBounds = GetEffectiveBounds();

			if (selected)
			{
				Rectangle effectiveOutline = Rectangle.Intersect(outlineRect, parent.GetEffectiveBounds());
				spriteBatch.Draw(pixel, effectiveOutline, effectiveOutline, outlineColor);
				spriteBatch.Draw(pixel, effectiveBounds, backgroundColor);
			}

			Rectangle effectiveBlock = new Rectangle(effectiveBounds.Location - bounds.Location, effectiveBounds.Size);
			block.DrawSimple(spriteBatch, ConvertUtils.PointToVector2(bounds.Location), scale, effectiveBlock);
			//spriteBatch.Draw(texture, ConvertUtils.PointToVector2(bounds.Location), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);


			if (hovered)
			{
				StructureEditor.SetTooltip(block.displayName);
			}
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
