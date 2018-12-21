using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Oceania_MG.Source.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	[DataContract]
	class World
	{
		private const int BIOME_DEPTH_SCALE = 50; //divides depth by this when doing biome calculation, to make it balance out with temp/life

		public const int SEA_LEVEL = 0;

		[DataMember]
		private string dir;

		[DataMember]
		private int seed;

		[DataMember]
		private Player player;

		public Generate generate;

		private Biome[] biomes;

		private Block[] blocks;

		private Dictionary<int, Dictionary<int, Chunk>> loadedChunks;

		public World(string name, int seed)
		{
			dir = "dat/" + name;
			this.seed = seed;
			generate = new Generate(seed);
			loadedChunks = new Dictionary<int, Dictionary<int, Chunk>>();

			string biomesJSON = File.ReadAllText("Content/Config/biomes.json");
			biomes = JsonConvert.DeserializeObject<Biomes>(biomesJSON).biomes;

			string blocksJSON = File.ReadAllText("Content/Config/blocks.json");
			blocks = JsonConvert.DeserializeObject<Blocks>(blocksJSON).blocks;
			int bid = 0;
			foreach (Block block in blocks)
			{
				block.id = bid;
				bid++;
			}
		}

		public void Load()
		{
			string path = dir + "/state";
			LoadState(path);
		}

		public void GenerateNew(Player.PlayerOptions playerOptions)
		{
			Directory.CreateDirectory(dir);
			Player player = new Player(new Vector2(0, 140), playerOptions);
			GenerateChunk(0, 0);
		}

		private void LoadState(string path)
		{
			//TODO
		}

		private void GenerateChunk(int x, int y)
		{
			Chunk chunk = new Chunk(x, y, this);
			chunk.Generate();
			loadedChunks[x][y] = chunk;
		}

		public Biome BiomeAt(int x, int y)
		{
			//TODO
			Tuple<float, float> properties = generate.Biome(x, y);
			return GetBiome(properties.Item1, properties.Item2, y);
		}

		public Biome GetBiome(float temperature, float liveliness, float depth)
		{
			Biome blendedBiome = new Biome(); //TODO lerp between biomes if just about halfway between them (so that heights smoothly transition)

			float minDistance = float.MaxValue;
			Biome bestBiome = new Biome();
			foreach (Biome biome in biomes)
			{
				Vector3 biomePos = new Vector3(biome.temperature, biome.liveliness, biome.depth / BIOME_DEPTH_SCALE);
				Vector3 currentPos = new Vector3(temperature, liveliness, depth / BIOME_DEPTH_SCALE);
				float distance = Vector3.Distance(biomePos, currentPos);
				if (distance < minDistance)
				{
					minDistance = distance;
					bestBiome = biome;
				}
			}

			//return blendedBiome;
			return bestBiome;
		}

		public Block GetBlock(string blockName)
		{
			return null; //TODO
		}
	}
}
