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

		[DataContract]
		public struct PlayerOptions
		{
			[DataMember]
			public HairColor hairColor;
			[DataMember]
			public HairLength hairLength;
			[DataMember]
			public BodyColor bodyColor;
			[DataMember]
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
		private const float MAX_SPEED = 0.2f;
		private const float ACCELERATION = 0.1f; //1 = instant
		private const float ZERO_SNAP_SPEED = 0.005f; //Speed below which to snap to 0 (to avoid pixel jittering)

		[DataMember]
		private PlayerOptions playerOptions;

		private Texture2D hairTexture;
		private Texture2D bodyTexture;
		private Texture2D tailTexture;

		public Player(World world, Vector2 position, PlayerOptions playerOptions) : base(world, "", position, MAX_HEALTH)
		{
			this.playerOptions = playerOptions;
		}

		protected override void LoadImage()
		{
			base.LoadImage();

			string hairColor = playerOptions.hairColor.ToString().ToLower() + "/" + playerOptions.hairLength.ToString().ToLower();
			hairTexture = Game.LoadImage("Images/player/hair/" + hairColor + "/idle");
			string bodyColor = playerOptions.bodyColor.ToString().ToLower();
			bodyTexture = Game.LoadImage("Images/player/body/" + bodyColor + "/idle");
			string tailColor = playerOptions.tailColor.ToString().ToLower();
			tailTexture = Game.LoadImage("Images/player/tail/" + tailColor + "/idle");
		}

		public override void Update(Input input, GameTime gameTime)
		{
			base.Update(input, gameTime);

			float inputX = 0;
			float inputY = 0;

			//Thumbstick movement takes precedence over digital key-based movement
			Vector2 inputMove = input.GetAxis(Input.Controls.Move);
			if (inputMove.X != 0 || inputMove.Y != 0)
			{
				inputX = inputMove.X;
				inputY = -inputMove.Y; //inverted in Monogame
			}
			else
			{
				if (input.ControlHeld(Input.Controls.Left))
				{
					inputX--;
				}
				if (input.ControlHeld(Input.Controls.Right))
				{
					inputX++;
				}
				if (input.ControlHeld(Input.Controls.Up))
				{
					inputY--;
				}
				if (input.ControlHeld(Input.Controls.Down))
				{
					inputY++;
				}
			}

			Vector2 goalVelocity = new Vector2(inputX, inputY) * MAX_SPEED;
			velocity = Vector2.Lerp(velocity, goalVelocity, ACCELERATION);

			//Snap to zero velocity to avoid very slow movement (causes pixel jittering)
			if (velocity.Length() < ZERO_SNAP_SPEED)
			{
				velocity = Vector2.Zero;
			}
		}

		public override void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw(graphicsDevice, spriteBatch, gameTime);

			//Use special center calculation instead of ConvertUtils.WorldToViewport(position.X, position.Y)
			//so that player is always rendered at the exact center, avoiding floating point issues
			float viewportX = (Game.GetWidth() - boundingBox.Width) / 2;
			float viewportY = (Game.GetHeight() - boundingBox.Height) / 2;
			Vector2 viewportPos = new Vector2(viewportX, viewportY);
			spriteBatch.Draw(tailTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
			spriteBatch.Draw(bodyTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
			spriteBatch.Draw(hairTexture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
		}
	}
}
