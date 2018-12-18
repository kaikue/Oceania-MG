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
	class EntityLiving : Entity
	{
		[DataMember]
		private uint health;

		[DataMember]
		private uint maxHealth;
		
		[DataMember]
		private float hurtTime = -1; //TODO: old "hurt" == hurtTime > 0

		[DataMember]
		private Vector2 knockback = new Vector2(0, 0);

		[DataMember]
		private DamageSource attack;

		public EntityLiving(string imageURL, Vector2 position, uint maxHealth) : base(imageURL, position)
		{
			this.health = maxHealth;
			this.maxHealth = maxHealth;
		}
	}
}
