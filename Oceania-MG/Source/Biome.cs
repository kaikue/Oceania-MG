using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	struct Biome
	{
		public string name;
		public float temperature;
		public float liveliness;
		//public float depth;
		public string baseBlock;
		public string surfaceBlock;
		public int minHeight;
		public int maxHeight;
		public string[] ores;
		public string[] structures;
		public int[] color;
	}
}
