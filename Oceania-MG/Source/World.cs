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
using System.Threading;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	[DataContract(IsReference = true)]
	class World
	{
		private const int CHUNK_LOAD_DISTANCE = 1; //(2 * this + 1) square of chunks is loaded surrounding player's chunk

		private const int BIOME_DEPTH_SCALE = 50; //divides depth by this when doing biome calculation, to make it balance out with temp/life

		public const int SEA_LEVEL = 0;

		private static readonly Color AIR_TINT = Color.White;
		private static readonly Color WATER_TINT = new Color(192, 224, 255);
		private static readonly Color ABYSS_TINT = Color.Black;
		private static readonly Color CORE_TINT = new Color(255, 192, 128);

		[DataMember]
		private string name;

		[DataMember]
		private int seed;

		[DataMember]
		private Player player;

		public Generate generate;

		private Biome[] biomes;
		private Dictionary<string, Ore> ores;

		private Dictionary<int, Block> blocks;
		private Dictionary<string, int> blockIDs;

		private HashSet<Chunk> loadedChunks;

		public World(string name, int seed)
		{
			this.name = name;
			this.seed = seed;
			generate = new Generate(seed);
			loadedChunks = new HashSet<Chunk>();

			string biomesJSON = File.ReadAllText("Content/Config/biomes.json");
			biomes = JsonConvert.DeserializeObject<Biomes>(biomesJSON).biomes;

			string oresJSON = File.ReadAllText("Content/Config/ores.json");
			Ore[] oreList = JsonConvert.DeserializeObject<Ores>(oresJSON).ores;
			ores = new Dictionary<string, Ore>();
			foreach(Ore ore in oreList)
			{
				ores[ore.name] = ore;
			}
			
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
		
		public void GenerateNew(Player.PlayerOptions playerOptions)
		{
			player = new Player(this, new Vector2(0, 1), playerOptions);
			LoadChunks(0, 0);
		}

		private void LoadChunks(int centerChunkX, int centerChunkY)
		{
			//save all chunks, since some may be unloaded
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.Save();
			}

			//load the new set of chunks surrounding center
			HashSet<Chunk> newLoadedChunks = new HashSet<Chunk>();
			for (int x = centerChunkX - CHUNK_LOAD_DISTANCE; x <= centerChunkX + CHUNK_LOAD_DISTANCE; x++)
			{
				for (int y = centerChunkY - CHUNK_LOAD_DISTANCE; y <= centerChunkY + CHUNK_LOAD_DISTANCE; y++)
				{
					//preferences:
					//1. use already loaded chunk
					//2. load chunk from disk
					//3. generate new chunk
					Chunk chunk = GetChunk(x, y);
					if (chunk == null)
					{
						//TODO: use Task for this
						chunk = Chunk.Load(x, y, this);
						if (chunk == null)
						{
							chunk = GenerateChunk(x, y);
						}
					}
					newLoadedChunks.Add(chunk);
				}
			}
			loadedChunks = newLoadedChunks;
		}

		private Chunk GenerateChunk(int x, int y)
		{
			Chunk chunk = new Chunk(x, y, this);
			chunk.Generate();
			return chunk;
		}

		public string GetDirectory()
		{
			return "Data/" + name;
		}

		public Player GetPlayer()
		{
			return player;
		}

		/// <summary>
		/// Returns the loaded chunk with according chunk coordinates, or null if the chunk is not loaded.
		/// </summary>
		public Chunk GetChunk(int chunkX, int chunkY)
		{
			return loadedChunks.FirstOrDefault(c => c.x == chunkX && c.y == chunkY);
		}

		/// <summary>
		/// Returns the block at a position in world coordinates.
		/// If the position is currently in an unloaded chunk, returns the "unknown" block.
		/// </summary>
		public Block BlockAt(int x, int y, bool background)
		{
			Tuple<Point, Vector2> chunkInfo = ConvertUtils.WorldToChunk(x, y);
			Point chunkPos = chunkInfo.Item1;
			int chunkX = chunkPos.X;
			int chunkY = chunkPos.Y;
			Vector2 subPos = chunkInfo.Item2;
			Chunk chunk = GetChunk(chunkX, chunkY);

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

		public Ore GetOre(string oreName)
		{
			return ores[oreName];
		}

		public Color GetLight(int x, int y)
		{
			//white above sea level, then blue -> black -> orange gradients
			if (y <= SEA_LEVEL)
			{
				return AIR_TINT;
			}
			else if (y < Generate.ABYSS_TOP)
			{
				return MathUtils.ColorGradient(y, Generate.LAND_TOP, Generate.ABYSS_TOP, WATER_TINT, ABYSS_TINT);
			}
			else if (y < Generate.ABYSS_BOTTOM)
			{
				return ABYSS_TINT;
			}
			else
			{
				return MathUtils.ColorGradient(y, Generate.ABYSS_BOTTOM, Generate.CORE_FULL, ABYSS_TINT, CORE_TINT);
			}
		}

		public IEnumerable<Entity> GetNearbyEntities(float x, float y)
		{
			//TODO: make this smarter by only looking at nearby chunks? have to watch out for really big entities
			return loadedChunks.Select(chunk => chunk.GetEntities()).Aggregate((e1, e2) => e1.Concat(e2));
		}

		public void Update(Input input, GameTime gameTime)
		{
			Point oldChunk = player.GetChunk();
			player.Update(input, gameTime);
			Point newChunk = player.GetChunk();
			if (oldChunk != newChunk)
			{
				LoadChunks(newChunk.X, newChunk.Y);
			}

			foreach (Chunk chunk in loadedChunks)
			{
				chunk.Update(input, gameTime);
			}
		}

		public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			//Render blocks first, then entities (so that entities aren't partially covered by other chunks' blocks
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.DrawBlocks(graphicsDevice, spriteBatch, gameTime);
			}
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.DrawEntities(graphicsDevice, spriteBatch, gameTime);
			}
			player.Draw(graphicsDevice, spriteBatch, gameTime);
		}
	}
}
