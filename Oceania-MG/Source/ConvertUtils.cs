using Microsoft.Xna.Framework;
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
	}
}
