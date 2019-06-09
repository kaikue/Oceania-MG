using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	class Resources
	{
		private Biome[] biomes;

		private Dictionary<string, Ore> ores;

		private Dictionary<string, Structure> structures;
		private List<string> structureNames;

		private Dictionary<int, Block> blocks;

		private Dictionary<string, int> blockIDs;

		private Microsoft.Xna.Framework.Game instance;

		public Resources(Microsoft.Xna.Framework.Game instance)
		{
			this.instance = instance;
		}

		public void LoadAll()
		{
			LoadOres();
			LoadStructures();
			LoadBiomes();
			LoadBlocks();
		}

		public void LoadOres()
		{
			string oresJSON = File.ReadAllText("Content/Config/ores.json");
			Ore[] oreList = JsonConvert.DeserializeObject<Ores>(oresJSON).ores;
			ores = new Dictionary<string, Ore>();
			foreach (Ore ore in oreList)
			{
				ores[ore.name] = ore;
			}
		}

		public void LoadStructures()
		{
			structures = new Dictionary<string, Structure>();
			string[] structureFiles = Directory.GetFiles("Content/Config/Structures", "*.json");
			foreach (string structureFile in structureFiles)
			{
				string structureJSON = File.ReadAllText(structureFile);
				Structure structure = JsonConvert.DeserializeObject<Structure>(structureJSON);
				string structureName = Path.GetFileNameWithoutExtension(structureFile);
				structure.name = structureName;
				structure.Process();
				structures[structureName] = structure;
			}

			//sort structures by size (width + height) so that larger structures take priority in generation
			structureNames = structures.OrderByDescending(structureInfo => {
				return structureInfo.Value.GetWidth() + structureInfo.Value.GetHeight();
			}).Select(structureInfo => structureInfo.Key).ToList();
		}

		public void LoadBiomes()
		{
			string biomesJSON = File.ReadAllText("Content/Config/biomes.json");
			biomes = JsonConvert.DeserializeObject<Biomes>(biomesJSON).biomes;

			foreach (Biome biome in biomes)
			{
				biome.LoadBackgrounds(this);
			}
		}

		public void LoadBlocks()
		{
			blocks = new Dictionary<int, Block>();
			blockIDs = new Dictionary<string, int>();
			string blocksJSON = File.ReadAllText("Content/Config/blocks.json");
			Block[] blocksList = JsonConvert.DeserializeObject<Blocks>(blocksJSON).blocks;

			//TODO: make sure this doesn't swap around block IDs unnecessarily (use hashCode or something for consistent block name -> block ID)

			int bid = 0;
			foreach (Block block in blocksList)
			{
				block.id = bid;
				bid++;

				blocks[block.id] = block;
				blockIDs[block.name] = block.id;

				block.LoadImage(LoadTexture(block.image));
			}
		}

		public Block GetBlock(string blockName)
		{
			return GetBlock(blockIDs[blockName]);
		}

		public Block GetBlock(int blockID)
		{
			return blocks[blockID];
		}

		public IEnumerable<Block> GetBlocks()
		{
			return blocks.Values;
		}

		public Ore GetOre(string oreName)
		{
			return ores[oreName];
		}

		public List<string> GetStructures()
		{
			return structureNames;
		}

		public Structure GetStructure(string structureName)
		{
			return structures[structureName];
		}

		public Biome[] GetBiomes()
		{
			return biomes;
		}

		public Biome GetBiome(float temperature, float liveliness, float depth)
		{
			float minDistance = float.MaxValue;
			float nextMinDistance = float.MaxValue;
			Biome bestBiome = null;
			Biome nextBestBiome = null;
			foreach (Biome biome in GetBiomes())
			{
				Vector3 biomePos = new Vector3(biome.temperature, biome.liveliness, biome.depth / Biome.DEPTH_SCALE);
				Vector3 currentPos = new Vector3(temperature, liveliness, depth / Biome.DEPTH_SCALE);
				float distance = Vector3.Distance(biomePos, currentPos);
				if (distance < minDistance)
				{
					nextMinDistance = minDistance;
					nextBestBiome = bestBiome;

					minDistance = distance;
					bestBiome = biome;
				}
				else if (distance < nextMinDistance)
				{
					nextMinDistance = distance;
					nextBestBiome = biome;
				}
			}

			if (nextBestBiome != null && Math.Abs(minDistance - nextMinDistance) < Biome.BLEND_DISTANCE)
			{
				//lerp between biomes if just about halfway between them (so that heights smoothly transition)
				//if minDistance == nextMinDistance: t = 0.5
				//if Math.Abs(minDistance - nextMinDistance) == 0.1f: t = 0
				float t = MathUtils.Lerp(0.5f, 0, Math.Abs(minDistance - nextMinDistance) / Biome.BLEND_DISTANCE);
				Biome blendedBiome = Biome.Lerp(bestBiome, nextBestBiome, t);
				return blendedBiome;
			}
			return bestBiome;
		}

		public Texture2D LoadTexture(string imageURL)
		{
			if (string.IsNullOrEmpty(imageURL))
			{
				return new Texture2D(instance.GraphicsDevice, GameplayState.BLOCK_SIZE, GameplayState.BLOCK_SIZE);
			}
			return instance.Content.Load<Texture2D>(imageURL);
		}
	}
}
