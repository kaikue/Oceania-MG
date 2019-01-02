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
	class EntityMoving : Entity
	{
		[DataMember]
		protected Vector2 moveDirection; //direction: -1, 0, 1

		[DataMember]
		protected Vector2 velocity;

		[DataMember]
		protected bool facingRight;

		[DataMember]
		protected bool blockCollisions; //Whether or not the entity should stop moving when hitting solid blocks

		public EntityMoving(World world, string imageURL, Vector2 position, bool facingRight = false, bool blockCollisions = true) : base(world, imageURL, position)
		{
			moveDirection = new Vector2(0, 0);
			velocity = new Vector2(0, 0);
			this.facingRight = facingRight;
			this.blockCollisions = blockCollisions;
		}
		
		private void MoveTentative(bool xAxis)
		{
			float axisVelocity = xAxis ? velocity.X : velocity.Y;
			float oldPos = xAxis ? position.X : position.Y;
			if (axisVelocity == 0)
			{
				//early out
				return;
			}

			//update position along this axis only
			if (xAxis)
			{
				position.X += axisVelocity;
			}
			else
			{
				position.Y += axisVelocity;
			}

			UpdateBoundingBoxAxis(xAxis);
			//move back if we hit something
			if (CheckBlockCollisions() && blockCollisions)
			{
				if (xAxis)
				{
					velocity.X = 0;
					position.X = oldPos;
				}
				else
				{
					velocity.Y = 0;
					position.Y = oldPos;
				}
				UpdateBoundingBoxAxis(xAxis);
			}

			//update image facing
			if (xAxis)
			{
				//axisVelocity is non-zero, since we checked that earlier
				facingRight = axisVelocity > 0;
			}
		}

		public override void Update(Input input, GameTime gameTime)
		{
			//try moving along x axis first, then y axis
			MoveTentative(true);
			MoveTentative(false);
			CheckEntityCollisions();
		}

		protected override bool IsFlipped()
		{
			return facingRight;
		}
	}
}
