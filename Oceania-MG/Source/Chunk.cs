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

		[DataMember]
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
			//TODO: multithreading

			//generate blocks & caves
			for (int y = 0; y < HEIGHT; y++)
			{
				for (int x = 0; x < WIDTH; x++)
				{
					Vector2 worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
					int worldX = (int)worldPos.X;
					int worldY = (int)worldPos.Y;
					Biome biome = world.BiomeAt(worldX, worldY);
					Tuple<float, float> noise = world.generate.Terrain(worldX, worldY, biome.minHeight, biome.maxHeight);
					SetBlockFromNoise(x, y, noise.Item2, false, biome);
					SetBlockFromNoise(x, y, noise.Item1, true, biome);
				}
			}

			//TODO: decorate with ores, structures, etc.
		}

		private void SetBlockFromNoise(int x, int y, float noise, bool background, Biome biome)
		{
			Vector2 worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
			int worldY = (int)worldPos.Y;
			//TODO variable thresholds from biome
			if (noise > -0.4)
			{
				SetBlockAt(x, y, biome.baseBlock, background);
			}
			else if (noise > -0.5)
			{
				SetBlockAt(x, y, biome.surfaceBlock, background);
			}
			else if (worldY > World.SEA_LEVEL)
			{
				SetBlockAt(x, y, world.GetBlock("water"), background);
			}
			else
			{
				SetBlockAt(x, y, world.GetBlock("air"), background);
			}
		}

		private void SetBlocksAt(int x, int y, Block block)
		{
			SetBlockAt(x, y, block, false);
			SetBlockAt(x, y, block, true);
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

		public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			//render blocks (background then foreground)
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					//TODO: occlusion check? would need to make sure there were no transparent pixels
					Vector2 viewportPos = ConvertUtils.ChunkToViewport(x, y, this.x, this.y);
					Vector2 worldPos = ConvertUtils.ChunkToWorld(x, y, this.x, this.y);
					int worldX = (int)worldPos.X;
					int worldY = (int)worldPos.Y;

					bool[] layers = { true, false };
					foreach (bool background in layers)
					{
						Block block = GetBlockAt(x, y, background);
						block.Draw(viewportPos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
					}
				}
			}

			//render entities
			foreach (Entity entity in entities)
			{
				entity.Draw(graphicsDevice, spriteBatch, gameTime);
			}
		}
	}
}
