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
		protected World world;

		[DataMember]
		protected string imageURL;

		protected Texture2D texture;

		[DataMember]
		protected Vector2 position;
		
		[DataMember]
		protected bool background;

		protected Rectangle boundingBox; //In unscaled pixel coordinates (1/GameplayState.BLOCK_SIZE of a world coordinate)

		public Entity(World world, string imageURL, Vector2 position, bool background = false)
		{
			this.world = world;
			this.imageURL = imageURL;
			this.position = position;
			this.background = background;

			LoadImage();
		}

		protected virtual void LoadImage()
		{
			texture = Game.LoadImage(imageURL);
			SetBoundingBox();
		}

		private void SetBoundingBox()
		{
			int width = texture == null ? ConvertUtils.WorldToPoint(1) : texture.Width;
			int height = texture == null ? ConvertUtils.WorldToPoint(1) : texture.Height;
			int x = ConvertUtils.WorldToPoint(position.X);
			int y = ConvertUtils.WorldToPoint(position.Y);
			boundingBox = new Rectangle(x, y, width, height);
		}

		protected void UpdateBoundingBoxAxis(bool xAxis)
		{
			if (xAxis)
			{
				boundingBox.X = ConvertUtils.WorldToPoint(position.X);
			}
			else
			{
				boundingBox.Y = ConvertUtils.WorldToPoint(position.Y);
			}
		}

		private bool CollidesWith(Rectangle otherRect)
		{
			return boundingBox.Intersects(otherRect);
		}

		protected bool CheckBlockCollisions()
		{
			int left = (int)Math.Floor(position.X);
			int top = (int)Math.Floor(position.Y);
			int width = (int)Math.Ceiling(ConvertUtils.PointToWorld(boundingBox.Width));
			int height = (int)Math.Ceiling(ConvertUtils.PointToWorld(boundingBox.Height));
			int right = left + width;
			int bottom = top + height;
			for (int x = left; x <= right; x++)
			{
				for (int y = top; y <= bottom; y++)
				{
					Block block = world.BlockAt(x, y, background);
					Rectangle blockRect = new Rectangle(ConvertUtils.WorldToPoint(x), ConvertUtils.WorldToPoint(y), ConvertUtils.WorldToPoint(1), ConvertUtils.WorldToPoint(1));
					if (block.solid && CollidesWith(blockRect))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void CheckEntityCollisions()
		{
			IEnumerable<Entity> entities = world.GetNearbyEntities(position.X, position.Y);
			foreach (Entity entity in entities)
			{
				if (CollidesWith(entity.boundingBox))
				{
					Collide(entity);
				}
			}
		}

		protected virtual void Collide(Entity other)
		{

		}

		public Vector2 GetChunk()
		{
			return ConvertUtils.WorldToChunk(position.X, position.Y).Item1;
		}

		public Point GetCenter()
		{
			return boundingBox.Center;
		}

		public virtual void Update(Input input, GameTime gameTime)
		{

		}

		public virtual bool IsFlipped()
		{
			return false;
		}

		public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (Game.IsDebugMode())
			{
				Point location = ConvertUtils.Vector2ToPoint(ConvertUtils.WorldToViewport(position.X, position.Y));
				Point size = boundingBox.Size;
				Rectangle destRect = new Rectangle(location, size);
				Console.WriteLine("drawing at " + destRect);
				spriteBatch.Draw(Game.GetPixelTexture(), destRect, Color.White);
			}

			if (texture == null) return;

			Vector2 viewportPos = ConvertUtils.WorldToViewport(position.X, position.Y);
			SpriteEffects effects = IsFlipped() ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(texture, viewportPos, null, GetColor(), 0, Vector2.Zero, GameplayState.SCALE, effects, 0);
		}
		
		public virtual Color GetColor()
		{
			return world.GetLight((int)position.X, (int)position.Y);
		}
	}
}
