using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649
//Fields are assigned by JSON conversion, so we don't worry about "field is never assigned" warnings.
namespace Oceania_MG.Source
{
	class Biome
	{
		public const int DEPTH_SCALE = 50; //divides depth by this when doing biome calculation, to make it balance out with temp/life
		public const float BLEND_DISTANCE = 0.1f; //how far apart to start blending biomes together

		public string name;
		public float temperature;
		public float liveliness;
		public float depth;
		public string baseBlock;
		public string surfaceBlock;
		public int minHeight = 64;
		public int maxHeight = 92;
		public string[] ores = new string[0];
		public string[] structures = new string[0];
		public int[] color = new int[] { 255, 255, 255 };
		public Color backgroundColor;

		public string[] backgrounds = { "Images/backgrounds/default/mid", "Images/backgrounds/default/far" };
		public Texture2D[] backgroundImages;

		public void LoadBackgrounds(Resources resources)
		{
			backgroundColor = new Color(color[0], color[1], color[2]);
			backgroundImages = new Texture2D[backgrounds.Length];
			for (int i = 0; i < backgrounds.Length; i++)
			{
				backgroundImages[i] = resources.LoadTexture(backgrounds[i]);
			}
		}

		public static Biome Lerp(Biome a, Biome b, float t)
		{
			Biome dominantBiome = t <= 0.5f ? a : b;
			Biome biome = new Biome
			{
				name = "Mix (" + a.name + ", " + b.name + ")",
				temperature = MathUtils.Lerp(a.temperature, b.temperature, t),
				liveliness = MathUtils.Lerp(a.liveliness, b.liveliness, t),
				depth = MathUtils.Lerp(a.depth, b.depth, t),
				baseBlock = dominantBiome.baseBlock,
				surfaceBlock = dominantBiome.surfaceBlock,
				minHeight = MathUtils.Lerp(a.minHeight, b.minHeight, t),
				maxHeight = MathUtils.Lerp(a.maxHeight, b.maxHeight, t),
				ores = dominantBiome.ores,
				structures = dominantBiome.structures,
				color = dominantBiome.color,
				backgroundColor = Color.Lerp(a.backgroundColor, b.backgroundColor, t)
			};
			return biome;
		}
	}

	struct Biomes
	{
		public Biome[] biomes;
	}
}
#pragma warning restore 0649
