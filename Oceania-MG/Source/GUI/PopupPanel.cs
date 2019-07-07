using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class PopupPanel : ContainerPanel
	{
		private GUIContainer parentContainer;

		public PopupPanel(Rectangle bounds, GUIContainer parentContainer, string label = null) : base(bounds, label)
		{
			this.parentContainer = parentContainer;
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (!hovered && (control == Input.Controls.LeftClick || control == Input.Controls.RightClick)) //TODO: more ways to close this, like ESC
			{
				Close();
			}
		}

		public void Close()
		{
			parentContainer.Remove(this);
		}
	}
}
