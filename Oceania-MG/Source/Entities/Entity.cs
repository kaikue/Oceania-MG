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
		private string imageURL;

		private Texture2D image;

		[DataMember]
		private Vector2 position;

		[DataMember]
		private Vector2 moveDirection; //direction: -1, 0, 1

		[DataMember]
		private Vector2 velocity;

		//TODO: facing (flip horizontal in sprite render)

		[DataMember]
		private bool isBackground;

		public Entity(string imageURL, Vector2 position, bool isBackground = false)
		{
			this.imageURL = imageURL;
			this.position = position;
			this.moveDirection = new Vector2(0, 0);
			this.velocity = new Vector2(0, 0);
			//TODO facing
			this.isBackground = isBackground;
		}

		private void LoadImage()
		{
			image = Game.LoadImage(imageURL);
		}
	}
}
