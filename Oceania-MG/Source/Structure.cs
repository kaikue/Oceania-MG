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
	class Structure
	{
		public string name;
		public float frequency; //What % of valid chunks to spawn in- between 0 (never) and 1 (always)
		public int minPerChunk = 1; //Inclusive
		public int maxPerChunk = 1; //Inclusive
		public string[][] blocksForeground;
		public string[][] blocksBackground;

		public Tuple<string, string> GetBlocksAt(int x, int y)
		{
			if (y < 0 || x < 0 || y >= GetHeight() || x >= GetWidth()) return null;
			return new Tuple<string, string>(GetBlockAt(x, y, true), GetBlockAt(x, y, false));
		}

		public string GetBlockAt(int x, int y, bool background)
		{
			string[][] blocks = background ? blocksBackground : blocksForeground;
			return blocks[y][x];
		}

		public int GetWidth()
		{
			return blocksForeground[0].Length;
		}

		public int GetHeight()
		{
			return blocksForeground.Length;
		}

		public bool CanSpawnAt(int worldX, int worldY, World world)
		{
			//TODO Check all anchors- terrain there is solid, biome there contains structure
			return true;
		}
	}

	struct Structures
	{
		public Structure[] structures;
	}
}
#pragma warning restore 0649
