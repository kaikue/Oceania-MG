﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class GUIContainer : GUIElement
	{
		private HashSet<GUIElement> elements;

		//TODO: maybe some stuff for text fields

		public GUIContainer(Rectangle bounds, GUIElement parent = null) : base(bounds, parent)
		{
			elements = new HashSet<GUIElement>();
		}
		
		public override void Update(Input input)
		{
			base.Update(input);

			HashSet<GUIElement> cachedElements = new HashSet<GUIElement>(elements); //Prevent concurrent modification exceptions

			foreach (GUIElement element in cachedElements)
			{
				element.Update(input);
			}

			foreach (Input.Controls pressedControl in input.PressedControls())
			{
				foreach (GUIElement element in cachedElements)
				{
					element.ControlPressed(pressedControl);
				}
			}

			foreach (Input.Controls pressedControl in input.HeldControls())
			{
				foreach (GUIElement element in cachedElements)
				{
					element.ControlHeld(pressedControl);
				}
			}

			foreach (Input.Controls releasedControl in input.ReleasedControls())
			{
				foreach (GUIElement element in cachedElements)
				{
					element.ControlReleased(releasedControl);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
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

		public void Remove(GUIElement element)
		{
			elements.Remove(element);
		}

		public IEnumerable<GUIElement> GetElements()
		{
			return elements;
		}
	}
}
