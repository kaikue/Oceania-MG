using Microsoft.Xna.Framework;
using Oceania_MG.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	[DataContract]
	class World
	{
		public const int HEIGHT = 256; //TODO: change this when doing vertical chunks- how open should lower chunks be? is there a limit?

		public const int SEA_LEVEL = 0;

		[DataMember]
		private string dir;

		[DataMember]
		private int seed;

		[DataMember]
		private Player player;

		public Generate generate;

		private Dictionary<int, Dictionary<int, Chunk>> loadedChunks;

		public World(string name, int seed)
		{
			dir = "dat/" + name;
			this.seed = seed;
			generate = new Generate(seed);
			loadedChunks = new Dictionary<int, Dictionary<int, Chunk>>();
		}

		public void Load()
		{
			string path = dir + "/state";
			LoadState(path);
		}

		public void GenerateNew(Player.PlayerOptions playerOptions)
		{
			Directory.CreateDirectory(dir);
			Player player = new Player(new Vector2(0, 140), playerOptions);
			GenerateChunk(0, 0);
		}

		private void LoadState(string path)
		{
			//TODO
		}

		private void GenerateChunk(int x, int y)
		{
			Chunk chunk = new Chunk(x, y);
			chunk.Generate(this);
			loadedChunks[x][y] = chunk;
		}

		public Biome GetBiome(int x, int y)
		{
			return null; //TODO
		}
	}
}
