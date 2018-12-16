using Microsoft.Xna.Framework;
using Oceania_MG.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	[DataContract]
	class Chunk
	{
		public const int WIDTH = 16;
		public const int HEIGHT = 16;

		[DataMember]
		int x;

		[DataMember]
		int y;

		[DataMember]
		int[][] blocksForeground;

		[DataMember]
		int[][] blocksBackground;

		[DataMember]
		List<Entity> entities;

		public Chunk(int x, int y)
		{
			this.x = x;
			this.y = y;
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

		public void Generate(World world)
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
					Biome biome = world.GetBiome(worldX, worldY);
					Tuple<float, float> noise = world.generate.Terrain(x, y, biome.minHeight, biome.maxHeight);
					SetBlockFromNoise(x, y, noise.Item2, false, biome, world);
					SetBlockFromNoise(x, y, noise.Item1, true, biome, world);
				}
			}

			//TODO: decorate with ores, structures, etc.
		}

		private void SetBlockFromNoise(int x, int y, float noise, bool background, Biome biome, World world)
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
				SetBlockAt(x, y, Block.water, background);
			}
			else
			{
				SetBlockAt(x, y, Block.air, background);
			}
		}

		private void SetBlocksAt(int x, int y, Block block)
		{
			SetBlockAt(x, y, block, false);
			SetBlockAt(x, y, block, true);
		}

		private void SetBlockAt(int x, int y, Block block, bool background)
		{
			int[][] blocks = background ? blocksBackground : blocksForeground;
			blocks[y][x] = block.id;
		}
	}
}
