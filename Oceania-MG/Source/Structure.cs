using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	[DataContract]
	class Structure
	{
		public struct Anchor
		{
			private const string ANY = "_any";
			private const string ANYSOLID = "_solid";

			public const string ANY_SATISFIED = "anySatisfied";

			public int x;
			public int y;
			private string foregroundBlock;
			private string backgroundBlock;


			public Anchor(int x, int y, string[] info)
			{
				this.x = x;
				this.y = y;
				foregroundBlock = info[0];
				backgroundBlock = info[1];
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

		[DataMember]
		public float frequency; //What % of valid chunks to spawn in- between 0 (never) and 1 (always)
		[DataMember]
		public int minPerChunk = 1; //Inclusive
		[DataMember]
		public int maxPerChunk = 1; //Inclusive
		[DataMember]
		public int attempts = 4 * Chunk.HEIGHT; //How many tries to fit this structure in a chunk
		/*[Obsolete("Remove this once structure editor is done")]
		public string[] layout;
		[Obsolete("Remove this once structure editor is done")]
		public Dictionary<string, string[]> blocks;*/
		public Dictionary<string, string[]> anchors;
		[DataMember]
		public string name;
		[DataMember]
		public string[][] blocksForeground;
		[DataMember]
		public string[][] blocksBackground;
		[DataMember]
		public HashSet<Anchor> strictAnchorsSet;
		[DataMember]
		public HashSet<Anchor> lenientAnchorsSet;

		public Structure()
		{
			//blocksForeground = new string[0][];
			//blocksBackground = new string[0][];

			blocksForeground = new string[1][];
			blocksBackground = new string[1][];
			blocksForeground[0] = new string[1] { "basalt" };
			blocksBackground[0] = new string[1] { "basalt" };

			strictAnchorsSet = new HashSet<Anchor>();
			lenientAnchorsSet = new HashSet<Anchor>();
		}

		/*[Obsolete("Remove this once structure editor is done")]
		public void Process()
		{
			blocksForeground = new string[layout.Length][];
			blocksBackground = new string[layout.Length][];
			strictAnchorsSet = new HashSet<Anchor>();
			lenientAnchorsSet = new HashSet<Anchor>();

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
						string[] anchorInfo = anchors[blockSymbol];
						Anchor anchor = new Anchor(j, i, anchorInfo);
						if (anchorInfo.Length > 2 && anchorInfo[2] == Anchor.ANY_SATISFIED)
						{
							Console.WriteLine("Adding lenient anchor " + blockSymbol + " for structure " + name);
							lenientAnchorsSet.Add(anchor);
						}
						else
						{
							Console.WriteLine("Adding strict anchor " + blockSymbol + " for structure " + name);
							strictAnchorsSet.Add(anchor);
						}
					}
				}
			}
		}*/

		public bool ContainsPosition(int x, int y)
		{
			//Check y first in case it's 0 (for some reason)
			return y >= 0 && x >= 0 && y < GetHeight() && x < GetWidth();
		}

		public Tuple<string, string> GetBlocksAt(int x, int y)
		{
			if (!ContainsPosition(x, y)) return null;
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
			if (blocksForeground.Length == 0) return 0;

			return blocksForeground[0].Length;
		}

		public int GetHeight()
		{
			return blocksForeground.Length;
		}

		public bool CanSpawnAt(int worldX, int worldY, World world)
		{
			//All of strict anchors must be true
			foreach (Anchor anchor in strictAnchorsSet)
			{
				if (!AnchorSatisfied(anchor, worldX, worldY, world))
				{
					return false;
				}
			}

			//Any of lenient anchors must be true
			bool satisfied = lenientAnchorsSet.Count() == 0;
			foreach (Anchor anchor in lenientAnchorsSet)
			{
				if (AnchorSatisfied(anchor, worldX, worldY, world))
				{
					satisfied = true;
				}
			}
			return satisfied;
		}

		private bool AnchorSatisfied(Anchor anchor, int worldX, int worldY, World world)
		{
			//Check that the structure can spawn in this biome
			Biome biome = world.GetBiomeAt(worldX, worldY);
			if (!biome.structures.Contains(name))
			{
				return false;
			}

			//Check that the anchor constraints are satisfied
			Tuple<Block, Block> terrain = world.GetTerrainAt(worldX + anchor.x, worldY + anchor.y);
			Block terrainBG = terrain.Item1;
			Block terrainFG = terrain.Item2;
			return anchor.IsValid(terrainBG, terrainFG);
		}
	}

	class SpawnedStructure
	{
		public int x;
		public int y;
		public Structure structure;

		public SpawnedStructure(int x, int y, Structure structure)
		{
			this.x = x;
			this.y = y;
			this.structure = structure;
		}
	}
}
