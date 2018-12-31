using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.Entities
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
			public HairColor hairColor;
			public HairLength hairLength;
			public BodyColor bodyColor;
			public TailColor tailColor;

			public PlayerOptions(HairColor hairColor, HairLength hairLength, BodyColor bodyColor, TailColor tailColor)
			{
				this.hairColor = hairColor;
				this.hairLength = hairLength;
				this.bodyColor = bodyColor;
				this.tailColor = tailColor;
			}
		}

		private const int MAX_HEALTH = 20;

		[DataMember]
		private PlayerOptions playerOptions;

		private Texture2D hairTexture;
		private Texture2D bodyTexture;
		private Texture2D tailTexture;

		public Player(Vector2 position, PlayerOptions playerOptions) : base("", position, MAX_HEALTH)
		{
			this.playerOptions = playerOptions;
			string hairColor = playerOptions.hairColor.ToString().ToLower() + "/" + playerOptions.hairLength.ToString().ToLower();
			hairTexture = Game.LoadImage("Images/player/hair/" + hairColor + "/idle");
			string bodyColor = playerOptions.bodyColor.ToString().ToLower();
			bodyTexture = Game.LoadImage("Images/player/body/" + bodyColor + "/idle");
			string tailColor = playerOptions.tailColor.ToString().ToLower();
			tailTexture = Game.LoadImage("Images/player/tail/" + tailColor + "/idle");
		}

		public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Vector2 viewportPos = ConvertUtils.WorldToViewport(position.X, position.Y);
			spriteBatch.Draw(tailTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
			spriteBatch.Draw(bodyTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
			spriteBatch.Draw(hairTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
		}
	}
}
