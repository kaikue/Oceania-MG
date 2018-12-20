using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649
//Fields are assigned by JSON conversion, so we don't worry about "field is never assigned" warnings.
namespace Oceania_MG.Source
{
	class Block
	{
		public const string CTM_NONE = "none"; //Always render full block
		public const string CTM_SOLID = "solid"; //Connect to any other solid block
		public const string CTM_SAMETYPE = "sametype"; //Only connect to blocks of same type

		public string name;
		public string displayName;
		public string image;
		public string[] description = new string[0];
		public bool solid = true;
		public bool breakable = true;
		public string connectedTexture = "none";
		public string entity = "";
		public int harvestLevel = 0;
		public int breakTime = 100;
		public int id;
	}

	struct Blocks
	{
		public Block[] blocks;
	}
#pragma warning restore 0649
}
