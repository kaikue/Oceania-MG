using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Oceania_MG.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	[DataContract]
	class World
	{
		public const int HEIGHT = 256; //TODO: change this when doing vertical chunks- how open should lower chunks be? is there a limit?

		public const int SEA_LEVEL = 0;

		[DataMember]
		private string dir;

		[DataMember]
		private int seed;

		[DataMember]
		private Player player;

		public Generate generate;

		private Biome[] biomes;

		private Dictionary<int, Dictionary<int, Chunk>> loadedChunks;

		class Biomes
		{
			public Biome[] biomes;
		}

		public World(string name, int seed)
		{
			dir = "dat/" + name;
			this.seed = seed;
			generate = new Generate(seed);
			loadedChunks = new Dictionary<int, Dictionary<int, Chunk>>();

			string biomesJSON = File.ReadAllLines("Content/Config/biomes.json").Aggregate((s1, s2) => s1 + s2);
			biomes = JsonConvert.DeserializeObject<Biomes>(biomesJSON).biomes;
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
			Vector2 properties = generate.Biome(x, y);
			return GetBiome(properties.X, properties.Y);
		}

		public Biome GetBiome(float temperature, float liveliness)
		{
			Biome blendedBiome = new Biome();

			//first just show diagram- no Generate() involved
			float minDistance = float.MaxValue;
			Biome bestBiome = new Biome();
			foreach (Biome biome in biomes)
			{
				Vector2 biomePos = new Vector2(biome.temperature, biome.liveliness);
				Vector2 currentPos = new Vector2(temperature, liveliness);
				float distance = Vector2.Distance(biomePos, currentPos);
				if (distance < minDistance)
				{
					minDistance = distance;
					bestBiome = biome;
				}
			}

			//return blendedBiome; //TODO
			return bestBiome;
		}

		public Block GetBlock(string blockName)
		{
			return null; //TODO
		}
	}
}
