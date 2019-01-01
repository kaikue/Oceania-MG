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
		private const float ISLAND_EXPANSION = 2.0f; //controls how much quickly islands taper off at the bottom (exponential)
		private const float CAVE_EXPANSION = 3.0f; //controls how much thicker caves are at the bottom of the cave layer (exponential)
		private const float CORE_CAVE_EXPANSION = 3.0f; //controls how much thicker caves are at the top of the core layer (exponential)
		private const float CORE_CAVE_THICKNESS = 2.0f; //controls how much thicker caves are in the core layer in general (linear)
		public const int ABYSS_DROPOFF = 30; //height in blocks of abyss layer background dropoff
		private const int BIOME_WIDTH_SCALE = 50; //controls horizontal scale of biomes
		private const int BIOME_HEIGHT_SCALE = 60; //controls vertical scale of biomes

		//Heights of world layers
		public const int ISLAND_BOTTOM = 20;
		public const int LAND_TOP = 50;
		public const int ABYSS_TOP = 300;
		public const int ABYSS_BOTTOM = 400;
		public const int CORE_FULL = 450;

		public int seed;
		private PerlinNoise terrainNoise2D;
		private PerlinNoise biomeTempNoise2D;
		private PerlinNoise biomeLifeNoise2D;
		private Dictionary<string, PerlinNoise> oreNoises2D;

		public Generate(int seed)
		{
			this.seed = seed;
			terrainNoise2D = new PerlinNoise(2, seed, 3);
			biomeTempNoise2D = new PerlinNoise(2, seed, 2, true);
			biomeLifeNoise2D = new PerlinNoise(2, -seed, 2, true);
			oreNoises2D = new Dictionary<string, PerlinNoise>();
		}

		public Tuple<float, float> Terrain(int x, int y, int minHeight, int maxHeight)
		{
			//regular 2D Perlin noise
			float[] point = new float[] { x / (float)TERRAIN_SCALE, y / (float)TERRAIN_SCALE };
			float noise = terrainNoise2D.Get(point);

			//apply gradient to make lower areas denser
			float noiseBG = MathUtils.GradientFilter(noise, y, minHeight, maxHeight);

			float noiseFG = noiseBG;

			//cut out caves if foreground
			float[] cavePoint = new float[] { x / (float)CAVE_SCALE, y / (float)CAVE_SCALE };
			float cave = terrainNoise2D.Get(cavePoint);
			
			if (y < ISLAND_BOTTOM)
			{
				//island gradient- no caves
				float islandG = MathUtils.Gradient(y, World.SEA_LEVEL, ISLAND_BOTTOM);
				float islandAdjust = 1 - (float)Math.Pow(islandG, ISLAND_EXPANSION);
				//if (y < World.SEA_LEVEL) cave = -1;
				//islandAdjust /= 1.5f;
				if (Math.Abs(cave * islandAdjust) < CAVE_CUTOFF)
				{
					noiseFG = -1;
					noiseBG = -1;
				}

				cave = -1; //don't apply caves to this step
			}
			else if (y < LAND_TOP)
			{
				//empty
				noiseFG = -1;
				noiseBG = -1;
			}
			else if (y < ABYSS_BOTTOM)
			{
				//caves open up into abyss
				float caveG = MathUtils.Gradient(y, minHeight, ABYSS_TOP);
				float caveAdjust = 1 - (float)Math.Pow(caveG, CAVE_EXPANSION);
				cave *= caveAdjust;

				//no background in abyss (except top/bottom and behind blocks)
				float abyssG = MathUtils.Gradient(y, ABYSS_TOP - ABYSS_DROPOFF, ABYSS_TOP);
				float abyssAdjust = (float)Math.Pow(abyssG, CAVE_EXPANSION);
				if (Math.Abs(noiseFG / abyssAdjust) < 0.5 || y > ABYSS_TOP) //use cave instead of noiseFG for smoother border
				{
					noiseBG = -1;
				}
			}
			else
			{
				//abyss ends in core
				float caveG = MathUtils.Gradient(y, ABYSS_BOTTOM, CORE_FULL);
				float caveAdjust = (float)Math.Pow(caveG, CORE_CAVE_EXPANSION) / CORE_CAVE_THICKNESS; //caves thicker overall
				cave *= caveAdjust;

				//abyss empty background bottom
				float abyssG = MathUtils.Gradient(y, CORE_FULL - ABYSS_DROPOFF, CORE_FULL);
				float abyssAdjust = (float)Math.Pow(1 - abyssG, CAVE_EXPANSION);
				if (Math.Abs(cave / abyssAdjust) < 0.7)
				{
					noiseBG = -1;
				}
			}

			if (Math.Abs(cave) < CAVE_CUTOFF) {
				noiseFG = -1;
			}

			return new Tuple<float, float>(noiseFG, noiseBG);
		}

		public float Ore(int x, int y, string oreName, float scale)
		{
			//Returns positive value (abs() of Perlin noise)
			if (!oreNoises2D.ContainsKey(oreName))
			{
				oreNoises2D[oreName] = new PerlinNoise(2, seed + oreName.GetHashCode(), 4);
			}

			float[] point = new float[] { x / scale, y / scale };
			float noise = oreNoises2D[oreName].Get(point);
			return Math.Abs(noise);
		}

		public Tuple<float, float> Biome(int x, int y)
		{
			float[] point = new float[] { x / (float)BIOME_WIDTH_SCALE, y / (float)BIOME_HEIGHT_SCALE };
			float temperatureNoise = biomeTempNoise2D.Get(point);
			float livelinessNoise = biomeLifeNoise2D.Get(point);
			return new Tuple<float, float>(temperatureNoise, livelinessNoise);
		}
	}
}
