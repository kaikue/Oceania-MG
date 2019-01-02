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
	class DamageSource : EntityMoving
	{
		[DataMember]
		private int damage;

		public DamageSource(int damage, World world, string imageURL, Vector2 position, bool facingRight = false, bool blockCollisions = true) : base(world, imageURL, position, facingRight, blockCollisions)
		{
			this.damage = damage;
		}
	}
}
