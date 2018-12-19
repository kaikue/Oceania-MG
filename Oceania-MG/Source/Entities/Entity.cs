using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		protected Texture2D image;

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
			image = Game.LoadImage(imageURL);
		}
	}
}
