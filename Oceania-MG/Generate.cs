using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	class Generate
	{
		private const int TERRAIN_SCALE = 30; //controls overall scale of terrain features
		private const int CAVE_SCALE = 40; //controls overall scale & thickness of caves
		private const float CAVE_CUTOFF = 0.05f; //controls thickness of caves
		private const float CAVE_EXPANSION = 0.7f; //controls how much thicker caves are at the bottom of the world

		public int seed;
		public PerlinNoise noise2d;

		public Generate(int seed)
		{
			this.seed = seed;
			noise2d = new PerlinNoise(2, seed, 3);
		}

		public Tuple<float, float> Terrain(int x, int y, int minHeight, int maxHeight)
		{
			//regular 2D Perlin noise
			float[] point = new float[] { x / (float)TERRAIN_SCALE, y / (float)TERRAIN_SCALE };
			float noise = noise2d.Get(point);

			//apply gradient to make lower areas denser
			float noiseBG = GradientFilter(noise, y, minHeight, maxHeight);

			float noiseFG = noiseBG;
			//cut out caves if foreground

			float[] cavePoint = new float[] { x / (float)CAVE_SCALE, y / (float)CAVE_SCALE };
			float cave = noise2d.Get(cavePoint);
			//caves get bigger as you get further down
			float caveG = Gradient(y, minHeight, World.HEIGHT);
			float caveAdjust = 1 - caveG * CAVE_EXPANSION;
			cave *= caveAdjust;

			if (-CAVE_CUTOFF < cave && cave < CAVE_CUTOFF) {
				noiseFG = -1;
			}

			return new Tuple<float, float>(noiseFG, noiseBG);
		}

		private static float Gradient(float y, float top, float bottom)
		{
			float g = (y - top) / (bottom - top);
			g = Math.Min(Math.Max(g, 0), 1);
			return g;
		}

		private static float GradientFilter(float n, float y, float top, float bottom)
		{
			//n: value to filter, from -1 to 1
			//y: amount along gradient
			//limits: top and bottom of gradient
			//returns new value of n from -1 to 1
			float g = Gradient(y, top, bottom);
			return (n + 1) * g - 1;
		}
	}
}
