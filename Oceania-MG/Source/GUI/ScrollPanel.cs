using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class ScrollPanel : ContainerPanel
	{
		private const int SCROLLBAR_WIDTH = 25;
		private const int SCROLL_BUTTON_HEIGHT = 25;

		private const int SCROLL_SPEED = 10;

		private int scrollOffset = 0;

		public ScrollPanel(Rectangle bounds, string label = null) : base(bounds, label)
		{
			Button scrollUpButton = new Button(new Rectangle(bounds.Width - SCROLLBAR_WIDTH, 0, SCROLLBAR_WIDTH, SCROLL_BUTTON_HEIGHT), "^", ScrollDown);
			Add(scrollUpButton);

			Button scrollDownButton = new Button(new Rectangle(bounds.Width - SCROLLBAR_WIDTH, bounds.Height - SCROLL_BUTTON_HEIGHT, SCROLLBAR_WIDTH, SCROLL_BUTTON_HEIGHT), "v", ScrollUp);
			Add(scrollDownButton);
		}

		protected override void RefreshBounds()
		{
			int oldOffsetY = offset.Y;
			base.RefreshBounds();
			offset.Y += scrollOffset;
			int deltaY = offset.Y - oldOffsetY;
			Point deltaOffset = new Point(0, deltaY);

			if (contents != null) //it will be null when first constructing
			{
				foreach (GUIElement element in contents.GetElements())
				{
					element.ApplyOffset(deltaOffset);
				}
			}
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (hovered)
			{
				if (control == Input.Controls.ScrollUp)
				{
					ScrollUp();
				}
				else if (control == Input.Controls.ScrollDown)
				{
					ScrollDown();
				}
			}
		}

		private void ScrollUp()
		{
			scrollOffset += SCROLL_SPEED;
			RefreshBounds();
		}

		private void ScrollDown()
		{
			scrollOffset -= SCROLL_SPEED;
			RefreshBounds();
		}
	}
}
