using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.Entities
{
	[DataContract(IsReference = true)]
	class EntityBlockOverlay : Entity
	{
		public EntityBlockOverlay(World world, string imageURL, Vector2 position) : base(world, imageURL, position)
		{
			//TODO: item spawned when block broken
		}
	}
}
