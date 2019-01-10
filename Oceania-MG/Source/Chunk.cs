﻿using Microsoft.Xna.Framework;
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
			for (int y = 0; y < HEIGHT; y++)
			{
				for (int x = 0; x < WIDTH; x++)
				{
					GenerateBlock(x, y);
				}
			}
		}

		private void GenerateBlock(int x, int y)
		{
			Point worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
			int worldX = worldPos.X;
			int worldY = worldPos.Y;
			Biome biome = world.BiomeAt(worldX, worldY);
			Tuple<float, float> noise = world.generate.Terrain(worldX, worldY, biome.minHeight, biome.maxHeight);

			string blockBG = GetBlockFromNoise(x, y, noise.Item1, true, biome);
			string blockFG = GetBlockFromNoise(x, y, noise.Item2, false, biome);

			blockFG = GetOreAt(worldX, worldY, biome, blockFG);
			
			Tuple<string, string> structureBlocks = GetStructureAt(worldX, worldY, biome, blockBG, blockFG);
			blockBG = structureBlocks.Item1;
			blockFG = structureBlocks.Item2;

			SetBlockAt(x, y, blockBG, true);
			SetBlockAt(x, y, blockFG, false);
		}

		private string GetBlockFromNoise(int worldX, int worldY, float terrainNoise, bool background, Biome biome)
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
			Block block = world.GetBlock(baseBlock);
			if (!block.solid) return baseBlock; //Only allow ore on solid terrain blocks

			foreach (string oreName in biome.ores)
			{
				Ore ore = world.GetOre(oreName);
				float oreNoise = world.generate.Ore(worldX, worldY, oreName, ore.scale);
				if (oreNoise > ore.cutoff)
				{
					return oreName;
				}
			}

			return baseBlock;
		}

		private Tuple<string, string> GetStructureAt(int worldX, int worldY, Biome biome, string baseBlockBG, string baseBlockFG)
		{
			Point currentChunk = ConvertUtils.WorldToChunk(worldX, worldY).Item1;
			int currentChunkX = currentChunk.X;
			int currentChunkY = currentChunk.Y;
			/*
			 * Currently only looks at structures that are valid for the current biome. This could lead to cases where a 
			 * structure is cut in half by a slice of another biome where it can't spawn, while both ends are in valid
			 * biomes.
			 * This could be fixed by looking at all structures for all biomes, but that would be slower, and result in
			 * structures frequently "leaking out" of the edges of the biomes where they can spawn.
			 */
			foreach (string structureName in biome.structures)
			{
				Tuple<string, string> structureBlocks = TrySpawnStructure(structureName, worldX, worldY, currentChunkX, currentChunkY);
				if (structureBlocks != null)
				{
					//Valid structure found
					return structureBlocks;
				}
			}

			//No structure found
			return new Tuple<string, string>(baseBlockBG, baseBlockFG);
		}

		private Tuple<string, string> TrySpawnStructure(string structureName, int worldX, int worldY, int currentChunkX, int currentChunkY)
		{
			Structure structure = world.GetStructure(structureName);
			Point minChunk = ConvertUtils.WorldToChunk(worldX - structure.GetWidth() + 1, worldY - structure.GetHeight() + 1).Item1;
			int minChunkX = minChunk.X;
			int minChunkY = minChunk.Y;
			for (int chunkX = currentChunkX; chunkX >= minChunkX; chunkX--)
			{
				for (int chunkY = currentChunkY; chunkY >= minChunkY; chunkY--)
				{
					Tuple<string, string> chunkResult = TrySpawnStructureInChunk(structure, worldX, worldY, chunkX, chunkY);
					if (chunkResult != null) return chunkResult;
				}
			}
			return null;
		}

		private Tuple<string, string> TrySpawnStructureInChunk(Structure structure, int worldX, int worldY, int chunkX, int chunkY)
		{
			int structuresPerChunk = world.generate.StructuresPerChunk(chunkX, chunkY, structure);
			if (structuresPerChunk == 0)
			{
				return null; //Early out, to avoid making full checks for each chunk
			}

			int structuresFound = 0;
			Point[] points = world.generate.ShufflePositions(chunkX, chunkY); //Randomly permute sub-chunk positions, seeded with chunkX, chunkY
			foreach (Point point in points)
			{
				Point structureWorldPos = ConvertUtils.ChunkToWorld(point.X, point.Y, chunkX, chunkY);
				if (structure.CanSpawnAt(structureWorldPos.X, structureWorldPos.Y, world))
				{
					//If the structure contains this position: return its foreground and background blocks there
					int xInStructure = worldX - structureWorldPos.X;
					int yInStructure = worldY - structureWorldPos.Y;
					Tuple<string, string> structureBlock = structure.GetBlocksAt(xInStructure, yInStructure);
					if (structureBlock != null) return structureBlock;

					structuresFound++;
					if (structuresFound == structuresPerChunk)
					{
						//All structures for this chunk generated, none contain this block
						return null;
					}
				}
			}
			//tried everything, no valid options
			return null;
		}
		
		private void SetBlockAt(int x, int y, string blockName, bool background)
		{
			Block block = world.GetBlock(blockName);
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
			return world.GetBlock(GetBlockIDAt(x, y, background));
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

					if (worldY > World.SEA_LEVEL)
					{
						Block water = world.GetBlock("water");
						water.Draw(viewportPos, graphicsDevice, spriteBatch, gameTime, false, worldX, worldY, world);
					}

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
