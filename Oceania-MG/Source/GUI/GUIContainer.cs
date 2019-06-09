using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class GUIContainer
	{
		private HashSet<GUIElement> elements;

		//TODO: maybe some stuff for text fields

		public GUIContainer()
		{
			elements = new HashSet<GUIElement>();
		}

		public void Update(Input input)
		{
			foreach (GUIElement element in elements)
			{
				element.Update(input);
			}

			foreach (Input.Controls pressedControl in input.PressedControls())
			{
				foreach (GUIElement element in elements)
				{
					element.ControlPressed(pressedControl);
				}
			}

			foreach (Input.Controls releasedControl in input.ReleasedControls())
			{
				foreach (GUIElement element in elements)
				{
					element.ControlReleased(releasedControl);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (GUIElement element in elements)
			{
				element.Draw(spriteBatch);
			}
		}

		public void Add(GUIElement element)
		{
			elements.Add(element);
		}
	}
}
