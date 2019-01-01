using Microsoft.Xna.Framework;
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
	}

	struct Biomes
	{
		public Biome[] biomes;
	}
}
#pragma warning restore 0649
