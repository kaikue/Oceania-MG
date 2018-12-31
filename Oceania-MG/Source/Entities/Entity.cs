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
	class Entity
	{

		[DataMember]
		protected string imageURL;

		protected Texture2D texture;

		[DataMember]
		protected Vector2 position;

		[DataMember]
		protected Vector2 moveDirection; //direction: -1, 0, 1

		[DataMember]
		protected Vector2 velocity;

		//TODO: facing (flip horizontal in sprite render)

		[DataMember]
		protected bool isBackground;

		public Entity(string imageURL, Vector2 position, bool isBackground = false)
		{
			this.imageURL = imageURL;
			this.position = position;
			moveDirection = new Vector2(0, 0);
			velocity = new Vector2(0, 0);
			//TODO facing
			this.isBackground = isBackground;
		}

		private void LoadImage()
		{
			texture = Game.LoadImage(imageURL);
		}

		public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (texture == null) return;

			Vector2 viewportPos = ConvertUtils.WorldToViewport(position.X, position.Y);
			spriteBatch.Draw(texture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
		}

		public virtual Color GetColor()
		{
			return Color.White;
		}
	}
}
