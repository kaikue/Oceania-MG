﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	abstract class GUIElement
	{
		protected Rectangle bounds;

		public static SpriteFont font;
		public static Texture2D pixel;
		public static int scale;

		protected bool hovered; //Set by the base Update() to be true iff the mouse is over top of this GUIElement.

		protected GUIElement(Rectangle bounds)
		{
			this.bounds = bounds;
		}

		/// <summary>
		/// Shift this GUIElement over by an offset (useful for when it is a sub-element of another GUIElement).
		/// </summary>
		/// <param name="offset">the amount to shift by</param>
		public void ApplyOffset(Point offset)
		{
			bounds.X += offset.X;
			bounds.Y += offset.Y;
			RefreshBounds();
		}

		/// <summary>
		/// Called when the bounds have been changed.
		/// </summary>
		protected virtual void RefreshBounds()
		{

		}

		/// <summary>
		/// Called when this GUIElement should be drawn to the SpriteBatch.
		/// </summary>
		/// <param name="spriteBatch">the SpriteBatch to draw onto</param>
		public abstract void Draw(SpriteBatch spriteBatch);

		/// <summary>
		/// Called when this GUIElement should be updated (approximately once every frame).
		/// </summary>
		/// <param name="mousePos">The position of the mouse in screen coordinates.</param>
		public virtual void Update(Input input)
		{
			hovered = bounds.Contains(input.GetMousePosition());
		}

		/// <summary>
		/// Called when the control is pressed (anywhere on the screen).
		/// In many cases, should be ignored if !this.hovered.
		/// </summary>
		/// <param name="control">The control that was pressed.</param>
		public virtual void ControlPressed(Input.Controls control)
		{

		}

		/// <summary>
		/// Called when the control is released (anywhere on the screen).
		/// In many cases, should be ignored if !this.hovered.
		/// </summary>
		/// <param name="control">The control that was released.</param>
		public virtual void ControlReleased(Input.Controls control)
		{

		}

		/// <summary>
		/// Returns the bounds of this GUIElement. Note that it may be a sub-element of another.
		/// </summary>
		public Rectangle GetBounds()
		{
			return bounds;
		}
	}
}