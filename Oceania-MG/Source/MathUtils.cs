using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	class MathUtils
	{
		public static float Gradient(float y, float top, float bottom)
		{
			float g = (y - top) / (bottom - top);
			g = Math.Min(Math.Max(g, 0), 1);
			return g;
		}

		public static float GradientFilter(float n, float y, float top, float bottom)
		{
			//n: value to filter, from -1 to 1
			//y: amount along gradient
			//limits: top and bottom of gradient
			//returns new value of n from -1 to 1
			float g = Gradient(y, top, bottom);
			return (n + 1) * g - 1;
		}

		public static Color ColorGradient(float y, float top, float bottom, Color topColor, Color bottomColor)
		{
			float t = Gradient(y, top, bottom);
			return Color.Lerp(topColor, bottomColor, t);
		}

		public static float Lerp(float a, float b, float t)
		{
			return (1 - t) * a + t * b;
		}

		public static int Lerp(int a, int b, float t)
		{
			return (int)Lerp((float)a, b, t);
		}
	}
}
