using Microsoft.Xna.Framework;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	class ConvertUtils
	{
		public static Vector2 ChunkToWorld(int xInChunk, int yInChunk, int chunkX, int chunkY)
		{
			return new Vector2(xInChunk + chunkX * Chunk.WIDTH,
								yInChunk + chunkY * Chunk.HEIGHT);
		}

		public static Vector2 WorldToViewport(float x, float y)
		{
			//TODO: get viewport?
			Vector2 viewport = new Vector2(0, 0);
			Vector2 pos = new Vector2(x, y);
			return pos * GameplayState.SCALED_BLOCK_SIZE - viewport;
		}

		public static Vector2 ChunkToViewport(int xInChunk, int yInChunk, int chunkX, int chunkY)
		{
			Vector2 worldPos = ChunkToWorld(xInChunk, yInChunk, chunkX, chunkY);
			return WorldToViewport(worldPos.X, worldPos.Y);
		}
	}
}
