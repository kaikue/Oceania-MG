using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class ContainerPanel : Panel
	{
		protected GUIContainer contents;
		protected Point offset;

		public ContainerPanel(Rectangle bounds, string label = null) : base(bounds, label)
		{
			contents = new GUIContainer(bounds);

			RefreshBounds();
		}

		protected override void RefreshBounds()
		{
			base.RefreshBounds();
			offset = bounds.Location; //TODO: move the offset over some to account for label?
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

		public void Add(GUIElement element)
		{
			element.ApplyOffset(offset);
			contents.Add(element);
		}
	}
}
