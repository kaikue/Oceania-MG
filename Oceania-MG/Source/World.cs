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

		public Resources resources;

		public Generate generate;

		private HashSet<Chunk> loadedChunks;

		public World(string name, int seed, Resources resources)
		{
			this.name = name;
			this.seed = seed;
			this.resources = resources;
			generate = new Generate(seed);
			loadedChunks = new HashSet<Chunk>();

			GenerateNew(new Player.PlayerOptions());
		}
		
		public void GenerateNew(Player.PlayerOptions playerOptions)
		{
			player = new Player(this, new Vector2(0, 80), playerOptions);
			Point playerChunk = player.GetChunk();
			LoadChunks(playerChunk.X, playerChunk.Y);
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
		public Block GetBlockAt(int x, int y, bool background)
		{
			Tuple<Point, Vector2> chunkInfo = ConvertUtils.WorldToChunk(x, y);
			Point chunkPos = chunkInfo.Item1;
			int chunkX = chunkPos.X;
			int chunkY = chunkPos.Y;
			Vector2 subPos = chunkInfo.Item2;
			Chunk chunk = GetChunk(chunkX, chunkY);

			if (chunk == null) return resources.GetBlock("unknown");
			return chunk.GetBlockAt((int)subPos.X, (int)subPos.Y, background);
		}

		public Biome GetBiomeAt(int x, int y)
		{
			Tuple<float, float> properties = generate.Biome(x, y);
			return resources.GetBiome(properties.Item1, properties.Item2, y);
		}

		public Tuple<Block, Block> GetTerrainAt(int x, int y)
		{
			Biome biome = GetBiomeAt(x, y);
			string blockBG = Chunk.GetTerrainAt(this, x, y, true, biome);
			string blockFG = Chunk.GetTerrainAt(this, x, y, false, biome);
			return new Tuple<Block, Block>(resources.GetBlock(blockBG), resources.GetBlock(blockFG));
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
			DrawBackground(graphicsDevice, spriteBatch, gameTime);

			//Render all blocks first, then entities (so that entities aren't partially covered by other chunks' blocks)
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.DrawBlocks(graphicsDevice, spriteBatch, gameTime);
			}
			foreach (Chunk chunk in loadedChunks)
			{
				chunk.DrawEntities(graphicsDevice, spriteBatch, gameTime);
			}
			//Render player on top of blocks and other entities
			player.Draw(graphicsDevice, spriteBatch, gameTime);
			//TODO: effects?
		}

		private void DrawBackground(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Vector2 playerPos = player.GetPosition();
			Biome biome = GetBiomeAt((int)playerPos.X, (int)playerPos.Y);

			graphicsDevice.Clear(biome.backgroundColor);

			//TODO make background look nicer
			/*int numLayers = biome.backgroundImages.Length;
			for (int layer = numLayers - 1; layer >= 0; layer--)
			{
				Texture2D backgroundImage = biome.backgroundImages[layer];
				
				float scaledWidth = (int)(backgroundImage.Width * GameplayState.SCALE);
				int repetitions = (int)Math.Ceiling(Game.GetWidth() / scaledWidth) + 1;
				float xParallax = (numLayers - layer) * 0.1f; //lower layers get more
				float yParallax = xParallax * 10;
				float playerX = ConvertUtils.WorldToPoint(playerPos.X);
				float xPos = (playerX * -xParallax) % scaledWidth;
				float yPos = (biome.depth + 80 - playerPos.Y) * yParallax; //TODO align to bottom better
				Vector2 position = new Vector2(xPos, yPos);
				for (int i = 0; i < repetitions; i++)
				{
					Vector2 offset = new Vector2((i - 1) * scaledWidth, 0);
					spriteBatch.Draw(backgroundImage, position + offset, null, Color.White, 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
				}
			}*/
		}
	}
}
