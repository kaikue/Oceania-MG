using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class Tooltip
	{
		private static Color bgColor = Color.White;
		private static Color outlineColor = Color.Black;
		private static Color textColor = Color.Black;

		public static void DrawTooltip(SpriteBatch spriteBatch, string text, Point pos)
		{
			//TODO: render background
			SpriteFont font = GUIElement.font;
			Texture2D pixel = GUIElement.pixel;
			int scale = GUIElement.scale;

			Point borderOffset = new Point(scale, scale);
			Vector2 size = font.MeasureString(text) * scale;
			Rectangle bgRect = new Rectangle(pos, ConvertUtils.Vector2ToPoint(size) + borderOffset);
			Rectangle outlineRect = new Rectangle(bgRect.Location - borderOffset, bgRect.Size + borderOffset + borderOffset + borderOffset);

			spriteBatch.Draw(pixel, outlineRect, outlineColor);
			spriteBatch.Draw(pixel, bgRect, bgColor);
			spriteBatch.DrawString(font, text, ConvertUtils.PointToVector2(pos + borderOffset), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}
	}
}
