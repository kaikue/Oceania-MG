using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	class Generate
	{
		private const int TERRAIN_SCALE = 30; //controls overall scale of terrain features
		private const int CAVE_SCALE = 40; //controls overall scale & thickness of caves
		private const float CAVE_CUTOFF = 0.05f; //controls thickness of caves
		private const float CAVE_EXPANSION = 0.7f; //controls how much thicker caves are at the bottom of the world
		private const int BIOME_WIDTH_SCALE = 30; //controls horizontal scale of biomes
		private const int BIOME_HEIGHT_SCALE = 60; //controls vertical scale of biomes

		public int seed;
		private PerlinNoise terrainNoise2D;
		private PerlinNoise biomeTempNoise2D;
		private PerlinNoise biomeLifeNoise2D;

		public Generate(int seed)
		{
			this.seed = seed;
			terrainNoise2D = new PerlinNoise(2, seed, 3);
			biomeTempNoise2D = new PerlinNoise(2, seed, 2, true);
			biomeLifeNoise2D = new PerlinNoise(2, -seed, 2, true);
		}

		public Tuple<float, float> Terrain(int x, int y, int minHeight, int maxHeight)
		{
			//regular 2D Perlin noise
			float[] point = new float[] { x / (float)TERRAIN_SCALE, y / (float)TERRAIN_SCALE };
			float noise = terrainNoise2D.Get(point);

			//apply gradient to make lower areas denser
			float noiseBG = GradientFilter(noise, y, minHeight, maxHeight);

			float noiseFG = noiseBG;
			//cut out caves if foreground

			float[] cavePoint = new float[] { x / (float)CAVE_SCALE, y / (float)CAVE_SCALE };
			float cave = terrainNoise2D.Get(cavePoint);
			//caves get bigger as you get further down
			float caveG = Gradient(y, minHeight, World.HEIGHT);
			float caveAdjust = 1 - caveG * CAVE_EXPANSION;
			cave *= caveAdjust;

			if (-CAVE_CUTOFF < cave && cave < CAVE_CUTOFF) {
				noiseFG = -1;
			}

			return new Tuple<float, float>(noiseFG, noiseBG);
		}

		public Tuple<float, float> Biome(int x, int y)
		{
			float[] point = new float[] { x / (float)BIOME_WIDTH_SCALE, y / (float)BIOME_HEIGHT_SCALE };
			float temperatureNoise = biomeTempNoise2D.Get(point);
			float livelinessNoise = biomeLifeNoise2D.Get(point);
			return new Tuple<float, float>(temperatureNoise, livelinessNoise);
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
