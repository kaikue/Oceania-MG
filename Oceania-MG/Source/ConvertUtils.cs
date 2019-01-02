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
			Vector2 viewport = Game.GetViewport();
			Vector2 pos = new Vector2(x, y);
			return (pos * GameplayState.BLOCK_SIZE - viewport) * GameplayState.SCALE;
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
		public static Tuple<Point, Vector2> WorldToChunk(float x, float y)
		{
			Point chunk = new Point((int)Math.Floor(x / Chunk.WIDTH), (int)Math.Floor(y / Chunk.HEIGHT));
			Vector2 subPos = new Vector2(x - chunk.X * Chunk.WIDTH, y - chunk.Y * Chunk.HEIGHT);
			return new Tuple<Point, Vector2>(chunk, subPos);
		}

		public static int WorldToPoint(float p)
		{
			return (int)Math.Round(p * GameplayState.BLOCK_SIZE);
		}

		public static float PointToWorld(int p)
		{
			return ((float)p) / GameplayState.BLOCK_SIZE;
		}

		public static Vector2 PointToVector2(Point p)
		{
			return new Vector2(p.X, p.Y);
		}

		public static Point Vector2ToPoint(Vector2 v)
		{
			return new Point((int)v.X, (int)v.Y);
		}
	}
}
