using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class PopupPanel : Panel
	{
		private GUIContainer contents;
		private Point offset;
		private GUIContainer parent;

		public PopupPanel(Rectangle bounds, GUIContainer parent, string label = null) : base(bounds, label)
		{
			this.parent = parent;
			contents = new GUIContainer();

			RefreshBounds();
		}

		protected override void RefreshBounds()
		{
			base.RefreshBounds();
			offset = bounds.Location; //TODO: move the offset over some?
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			contents.Draw(spriteBatch);
		}

		public override void Update(Input input)
		{
			base.Update(input);

			contents.Update(input);
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (!hovered && (control == Input.Controls.LeftClick || control == Input.Controls.RightClick)) //TODO: more ways to close this, like ESC
			{
				Close();
			}
		}

		public void Add(GUIElement element)
		{
			element.ApplyOffset(offset);
			contents.Add(element);
		}

		public void Close()
		{
			parent.Remove(this);
		}
	}
}
