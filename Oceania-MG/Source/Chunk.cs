using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Oceania_MG.Source.Entities;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	[DataContract(IsReference = true)]
	class Chunk
	{
		public const int WIDTH = 16;
		public const int HEIGHT = 16;

		[DataMember]
		public int x;

		[DataMember]
		public int y;

		[DataMember]
		private int[][] blocksForeground;

		[DataMember]
		private int[][] blocksBackground;

		[DataMember]
		private List<Entity> entities;

		private World world;

		private List<SpawnedStructure> structures;

		public Chunk(int x, int y, World world)
		{
			this.x = x;
			this.y = y;
			this.world = world;
			blocksForeground = EmptyBlockArray();
			blocksBackground = EmptyBlockArray();
			entities = new List<Entity>();
		}

		private static int[][] EmptyBlockArray()
		{
			int[][] blocks = new int[HEIGHT][];
			for (int i = 0; i < HEIGHT; i++)
			{
				blocks[i] = new int[WIDTH];
			}
			return blocks;
		}

		public void Generate()
		{
			ProcessStructures();
			for (int y = 0; y < HEIGHT; y++)
			{
				for (int x = 0; x < WIDTH; x++)
				{
					GenerateBlock(x, y);
				}
			}
		}

		private void ProcessStructures()
		{
			structures = new List<SpawnedStructure>();
			//Find all structures that overlap this chunk and can generate
			IEnumerable<string> structuresList = world.resources.GetStructures();
			Point worldPos = ConvertUtils.ChunkToWorld(0, 0, x, y);
			int worldX = worldPos.X;
			int worldY = worldPos.Y;
			foreach (string structureName in structuresList)
			{
				Structure structure = world.resources.GetStructure(structureName);
				Point minChunk = ConvertUtils.WorldToChunk(worldX - structure.GetWidth() + 1, worldY - structure.GetHeight() + 1).Item1;
				int minChunkX = minChunk.X;
				int minChunkY = minChunk.Y;
				for (int chunkX = x; chunkX >= minChunkX; chunkX--)
				{
					for (int chunkY = y; chunkY >= minChunkY; chunkY--)
					{
						List<SpawnedStructure> spawnedStructures = TryPlaceStructuresInChunk(structure, chunkX, chunkY);
						structures.AddRange(spawnedStructures);
					}
				}
			}
		}

		private List<SpawnedStructure> TryPlaceStructuresInChunk(Structure structure, int chunkX, int chunkY)
		{
			List<SpawnedStructure> spawnedStructures = new List<SpawnedStructure>();

			int structuresPerChunk = world.generate.StructuresPerChunk(chunkX, chunkY, structure);
			if (structuresPerChunk == 0)
			{
				return spawnedStructures;
			}

			int structuresFound = 0;
			Point point = world.generate.Position(chunkX, chunkY, structuresFound);
			for (int i = 0; i < structure.attempts; i++)
			{
				Point structureWorldPos = ConvertUtils.ChunkToWorld(point.X, point.Y, chunkX, chunkY);
				if (structure.CanSpawnAt(structureWorldPos.X, structureWorldPos.Y, world))
				{
					spawnedStructures.Add(new SpawnedStructure(structureWorldPos.X, structureWorldPos.Y, structure));

					structuresFound++;
					if (structuresFound == structuresPerChunk)
					{
						//All structures for this chunk generated
						break;
					}
					point = world.generate.Position(chunkX, chunkY, structuresFound);
				}
				else
				{
					point = NextPoint(point);
				}
			}
			
			return spawnedStructures;
		}

		private static Point NextPoint(Point point)
		{
			//Go in columns, since many structures are restricted to the surface
			int newX = point.X;
			int newY = point.Y + 1;
			if (newY >= HEIGHT)
			{
				newY -= HEIGHT;
				newX++;
				if (newX >= WIDTH)
				{
					newX -= WIDTH;
				}
			}
			return new Point(newX, newY);
		}

		private void GenerateBlock(int x, int y)
		{
			Point worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
			int worldX = worldPos.X;
			int worldY = worldPos.Y;

			Biome biome = world.GetBiomeAt(worldX, worldY);
			string blockBG = GetTerrainAt(world, worldX, worldY, true, biome);
			string blockFG = GetTerrainAt(world, worldX, worldY, false, biome);

			blockFG = GetOreAt(worldX, worldY, biome, blockFG);
			
			Tuple<string, string> structureBlocks = GetStructureAt(worldX, worldY, blockBG, blockFG);
			if (structureBlocks.Item1 != null) blockBG = structureBlocks.Item1;
			if (structureBlocks.Item2 != null) blockFG = structureBlocks.Item2;

			SetBlockAt(x, y, blockBG, true);
			SetBlockAt(x, y, blockFG, false);
		}

		public static string GetTerrainAt(World world, int worldX, int worldY, bool background, Biome biome)
		{
			Tuple<float, float> noise = world.generate.Terrain(worldX, worldY, biome.minHeight, biome.maxHeight);
			return GetBlockFromNoise(worldX, worldY, background ? noise.Item1 : noise.Item2, background, biome);
		}

		private static string GetBlockFromNoise(int worldX, int worldY, float terrainNoise, bool background, Biome biome)
		{
			//TODO variable thresholds from biome
			if (terrainNoise > -0.5)
			{
				string block = terrainNoise > -0.4 ? biome.baseBlock : biome.surfaceBlock;
				return block;
			}
			else if (worldY > World.SEA_LEVEL)
			{
				return "water";
			}
			else
			{
				return "air";
			}
		}

		private string GetOreAt(int worldX, int worldY, Biome biome, string baseBlock)
		{
			Block block = world.resources.GetBlock(baseBlock);
			if (!block.solid) return baseBlock; //Only allow ore on solid terrain blocks

			foreach (string oreName in biome.ores)
			{
				Ore ore = world.resources.GetOre(oreName);
				float oreNoise = world.generate.Ore(worldX, worldY, oreName, ore.scale);
				if (oreNoise > ore.cutoff)
				{
					return oreName;
				}
			}

			return baseBlock;
		}

		private Tuple<string, string> GetStructureAt(int worldX, int worldY, string baseBlockBG, string baseBlockFG)
		{
			foreach (SpawnedStructure spawnedStructure in structures)
			{
				int xInStructure = worldX - spawnedStructure.x;
				int yInStructure = worldY - spawnedStructure.y;
				Structure structure = spawnedStructure.structure;
				if (structure.ContainsPosition(xInStructure, yInStructure))
				{
					Tuple<string, string> structureBlock = structure.GetBlocksAt(xInStructure, yInStructure);
					if (structureBlock != null)
					{
						return structureBlock;
					}
				}
			}

			//No structure found
			return new Tuple<string, string>(baseBlockBG, baseBlockFG);
		}

		private void SetBlockAt(int x, int y, string blockName, bool background)
		{
			Block block = world.resources.GetBlock(blockName);
			SetBlockAt(x, y, block, background);
		}

		private void SetBlockAt(int x, int y, Block block, bool background)
		{
			int[][] blocks = background ? blocksBackground : blocksForeground;
			blocks[y][x] = block.id;
		}

		private int GetBlockIDAt(int x, int y, bool background)
		{
			int[][] blocks = background ? blocksBackground : blocksForeground;
			return blocks[y][x];
		}

		public Block GetBlockAt(int x, int y, bool background)
		{
			return world.resources.GetBlock(GetBlockIDAt(x, y, background));
		}

		public IEnumerable<Entity> GetEntities()
		{
			return entities;
		}

		public void Update(Input input, GameTime gameTime)
		{
			HashSet<Entity> toRemove = new HashSet<Entity>();

			foreach (Entity entity in entities)
			{
				entity.Update(input, gameTime);
				Point idealChunk = entity.GetChunk();
				int idealChunkX = idealChunk.X;
				int idealChunkY = idealChunk.Y;
				if (idealChunkX != x || idealChunkY != y)
				{
					Chunk newChunk = world.GetChunk(idealChunkX, idealChunkY);
					if (newChunk != null)
					{
						newChunk.entities.Add(entity);
						toRemove.Add(entity);
					}
				}
			}

			foreach (Entity entity in toRemove)
			{
				entities.Remove(entity);
			}
		}

		public void DrawBlocks(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					//TODO: occlusion check? would need to make sure there were no transparent pixels
					Vector2 viewportPos = ConvertUtils.ChunkToViewport(x, y, this.x, this.y);
					Point worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
					int worldX = worldPos.X;
					int worldY = worldPos.Y;

					bool[] layers = { true, false };
					foreach (bool background in layers)
					{
						Block block = GetBlockAt(x, y, background);
						block.Draw(viewportPos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
					}
				}
			}
		}

		public void DrawEntities(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			foreach (Entity entity in entities)
			{
				entity.Draw(graphicsDevice, spriteBatch, gameTime);
			}
		}

		private void SetWorld(World world)
		{
			this.world = world;
			foreach (Entity entity in entities)
			{
				entity.SetWorld(world);
			}
		}

		private static string GetFilename(int x, int y, World world)
		{
			return world.GetDirectory() + "/" + x + "_" + y + ".chunk";
		}

		/// <summary>
		/// Saves the chunk to disk.
		/// </summary>
		public void Save()
		{
			Task.Run(() => SaveLoad.Save(this, GetFilename(x, y, world)));
		}

		/// <summary>
		/// Returns the chunk loaded on disk at chunk coordinates (x, y), or null if it does not exist.
		/// </summary>
		public static Chunk Load(int x, int y, World world)
		{
			Chunk chunk = SaveLoad.Load<Chunk>(GetFilename(x, y, world));
			if (chunk != null)
			{
				chunk.SetWorld(world);
			}
			return chunk;
		}
	}
}
