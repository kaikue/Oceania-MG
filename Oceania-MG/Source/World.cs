using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Oceania_MG.Source.Entities;
using Oceania_MG.Source.States;
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

		private Dictionary<int, Block> blocks;
		private Dictionary<string, int> blockIDs;

		private HashSet<Chunk> loadedChunks;

		public World(string name, int seed)
		{
			dir = "dat/" + name;
			this.seed = seed;
			generate = new Generate(seed);
			loadedChunks = new HashSet<Chunk>();

			string biomesJSON = File.ReadAllText("Content/Config/biomes.json");
			biomes = JsonConvert.DeserializeObject<Biomes>(biomesJSON).biomes;

			blocks = new Dictionary<int, Block>();
			blockIDs = new Dictionary<string, int>();
			string blocksJSON = File.ReadAllText("Content/Config/blocks.json");
			Block[] blocksList = JsonConvert.DeserializeObject<Blocks>(blocksJSON).blocks;
			int bid = 0;
			foreach (Block block in blocksList)
			{
				block.id = bid;
				bid++;

				blocks[block.id] = block;
				blockIDs[block.name] = block.id;

				block.LoadImage();
			}

			GenerateNew(new Player.PlayerOptions());
		}

		public void Load()
		{
			string path = dir + "/state";
			LoadState(path);
		}

		public void GenerateNew(Player.PlayerOptions playerOptions)
		{
			Directory.CreateDirectory(dir);
			player = new Player(new Vector2(0, 140), playerOptions);
			GenerateChunk(0, 0);
			GenerateChunk(0, 1);
			GenerateChunk(1, 0);
			GenerateChunk(1, 1);
		}

		private void LoadState(string path)
		{
			//TODO
		}

		private void GenerateChunk(int x, int y)
		{
			Chunk chunk = new Chunk(x, y, this);
			chunk.Generate();
			loadedChunks.Add(chunk);
		}

		/// <summary>
		/// Returns the block at a position in world coordinates.
		/// If the position is currently in an unloaded chunk, returns the "unknown" block.
		/// </summary>
		public Block BlockAt(int x, int y, bool background)
		{
			Tuple<Vector2, Vector2> chunkInfo = ConvertUtils.WorldToChunk(x, y);
			Vector2 chunkPos = chunkInfo.Item1;
			int chunkX = (int)chunkPos.X;
			int chunkY = (int)chunkPos.Y;
			Vector2 subPos = chunkInfo.Item2;
			Chunk chunk = loadedChunks.FirstOrDefault(c => c.x == chunkX && c.y == chunkY);

			if (chunk == null) return GetBlock("unknown");
			return chunk.GetBlockAt((int)subPos.X, (int)subPos.Y, background);
		}

		public Biome BiomeAt(int x, int y)
		{
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
			return GetBlock(blockIDs[blockName]);
		}

		public Block GetBlock(int blockID)
		{
			return blocks[blockID];
		}

		public Color GetLight(int x, int y)
		{
			//TODO white above sea level, then blue -> black -> orange gradients
			return Color.White;
		}

		public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.Draw(graphicsDevice, spriteBatch, gameTime);
			}
			player.Draw(graphicsDevice, spriteBatch, gameTime);
		}
	}
}
