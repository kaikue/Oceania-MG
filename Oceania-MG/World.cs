using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	class World
	{
		private string dir;

		public World(string name)
		{
			dir = "dat/" + name;
		}

		public void Load()
		{
			string path = dir + "/state";
			LoadState(path);
		}

		public void Generate(int seed, Player playerOptions)
		{
			//TODO
		}

		private void LoadState(string path)
		{
			//TODO
		}
	}
}
