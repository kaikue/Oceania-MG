using Microsoft.Xna.Framework;
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

		[DataMember]
		private string dir;

		[DataMember]
		private int seed;

		[DataMember]
		private Player player;

		private Generate generate;

		public World(string name)
		{
			dir = "dat/" + name;
		}

		public void Load()
		{
			string path = dir + "/state";
			LoadState(path);
		}

		public void Generate(int seed, Player.PlayerOptions playerOptions)
		{
			Directory.CreateDirectory(dir);
			Player player = new Player(new Vector2(0, 140), playerOptions);
			this.seed = seed;

		}

		private void LoadState(string path)
		{
			//TODO
		}
	}
}
