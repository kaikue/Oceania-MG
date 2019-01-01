using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649
//Fields are assigned by JSON conversion, so we don't worry about "field is never assigned" warnings.
namespace Oceania_MG.Source
{
	class Ore
	{
		public string name; //should be the same as the block name
		public int frequency; //controls number of clusters (0 = none, 1 = normal, 2 = more, 3 = tons, etc.)
		public float cutoff; //controls size of clusters (higher = smaller clusters)
		public float scale; //controls spacing & size of clusters (higher = more spread apart and larger)
	}

	struct Ores
	{
		public Ore[] ores;
	}
}
#pragma warning restore 0649
