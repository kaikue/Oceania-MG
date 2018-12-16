using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG
{
	[DataContract(IsReference = true)]
	class Player : EntityLiving
	{
		public enum HairColor
		{
			Yellow,
			Red,
			Brown,
			Black,
			Gray,
			White,
			Green,
			Blue,
			None
		}

		public enum HairLength
		{
			Short,
			Medium,
			Long
		}

		public enum BodyColor
		{
			Peach,
			Tan,
			Brown,
			White,
			Purple,
			Black
		}

		public enum TailColor
		{
			Blue,
			Green,
			Red,
			Purple,
			Orange,
			Black
		}

		public struct PlayerOptions
		{
			HairColor hairColor;
			HairLength hairLength;
			BodyColor bodyColor;
			TailColor tailColor;

			public PlayerOptions(HairColor hairColor, HairLength hairLength, BodyColor bodyColor, TailColor tailColor)
			{
				this.hairColor = hairColor;
				this.hairLength = hairLength;
				this.bodyColor = bodyColor;
				this.tailColor = tailColor;
			}
		}

		private const uint MAX_HEALTH = 20;

		[DataMember]
		private PlayerOptions playerOptions;

		public Player(Vector2 position, PlayerOptions playerOptions) : base("", position, MAX_HEALTH)
		{
			this.playerOptions = playerOptions;

		}
	}
}
