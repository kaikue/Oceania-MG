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

		/// <summary>
		/// Returns (chunk, position within chunk)
		/// </summary>
		/// <param name="x">x in world coordinates</param>
		/// <param name="y">y in world coordinates</param>
		public static Tuple<Vector2, Vector2> WorldToChunk(float x, float y)
		{
			Vector2 chunk = new Vector2((float)Math.Floor(x / Chunk.WIDTH), (float)Math.Floor(y / Chunk.HEIGHT));
			Vector2 subPos = new Vector2(x - chunk.X * Chunk.WIDTH, y - chunk.Y * Chunk.HEIGHT);
			return new Tuple<Vector2, Vector2>(chunk, subPos);
		}
	}
}
