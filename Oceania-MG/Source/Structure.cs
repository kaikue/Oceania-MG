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
		private struct Anchor
		{
			private const string ANY = "_any";
			private const string ANYSOLID = "_solid";

			public int x;
			public int y;
			private string foregroundBlock;
			private string backgroundBlock;

			public Anchor(int x, int y, string foregroundBlock, string backgroundBlock)
			{
				this.x = x;
				this.y = y;
				this.foregroundBlock = foregroundBlock;
				this.backgroundBlock = backgroundBlock;
			}

			public bool IsValid(Block terrainBG, Block terrainFG)
			{
				return IsValidLayer(terrainBG, backgroundBlock) && IsValidLayer(terrainFG, foregroundBlock);
			}

			private bool IsValidLayer(Block block, string requirement)
			{
				if (requirement == ANY)
				{
					return true;
				}
				else if (requirement == ANYSOLID)
				{
					return block.solid;
				}
				else
				{
					return block.name == requirement;
				}
			}
		}

		public float frequency; //What % of valid chunks to spawn in- between 0 (never) and 1 (always)
		public int minPerChunk = 1; //Inclusive
		public int maxPerChunk = 1; //Inclusive
		public int attempts = Chunk.HEIGHT; //How many tries to fit this structure in a chunk
		public string[] layout;
		public Dictionary<string, string[]> blocks;
		public Dictionary<string, string[]> anchors;
		public string name;
		private string[][] blocksForeground;
		private string[][] blocksBackground;
		private HashSet<Anchor> anchorsSet;

		public void Process()
		{
			blocksForeground = new string[layout.Length][];
			blocksBackground = new string[layout.Length][];
			anchorsSet = new HashSet<Anchor>();

			for (int i = 0; i < layout.Length; i++)
			{
				string line = layout[i];
				blocksForeground[i] = new string[line.Length];
				blocksBackground[i] = new string[line.Length];
				for (int j = 0; j < line.Length; j++)
				{
					string blockSymbol = line[j].ToString();
					if (blocks.ContainsKey(blockSymbol))
					{
						string[] blockPair = blocks[blockSymbol];
						blocksForeground[i][j] = blockPair[0];
						blocksBackground[i][j] = blockPair[1];
					}
					if (anchors.ContainsKey(blockSymbol))
					{
						string[] anchorPair = anchors[blockSymbol];
						anchorsSet.Add(new Anchor(j, i, anchorPair[0], anchorPair[1]));
					}
				}
			}
		}

		public Tuple<string, string> GetBlocksAt(int x, int y)
		{
			if (y < 0 || x < 0 || y >= GetHeight() || x >= GetWidth()) return null;
			string blockBG = GetBlockAt(x, y, true);
			string blockFG = GetBlockAt(x, y, false);
			if (blockBG == null && blockFG == null) return null; //If both are empty, allow other structures to generate here
			return new Tuple<string, string>(blockBG, blockFG);
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
			foreach (Anchor anchor in anchorsSet)
			{
				Tuple<Block, Block> terrain = world.GetTerrainAt(worldX + anchor.x, worldY + anchor.y);
				Block terrainBG = terrain.Item1;
				Block terrainFG = terrain.Item2;
				if (!anchor.IsValid(terrainBG, terrainFG))
				{
					return false;
				}
			}
			return true;
		}
	}
}
#pragma warning restore 0649
