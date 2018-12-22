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
		protected int health;

		[DataMember]
		protected int maxHealth;

		[DataMember]
		protected float hurtTime = 0; //TODO: old "hurt" == hurtTime > 0

		[DataMember]
		protected Vector2 knockback = new Vector2(0, 0);

		[DataMember]
		protected DamageSource attack;

		public EntityLiving(string imageURL, Vector2 position, int maxHealth) : base(imageURL, position)
		{
			health = maxHealth;
			this.maxHealth = maxHealth;
		}

		private bool IsHurt()
		{
			return hurtTime > 0;
		}

		public override Color GetColor()
		{
			if (IsHurt()) return Color.Red;
			return base.GetColor();
		}
	}
}
