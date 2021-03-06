﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 0649
//Fields are assigned by JSON conversion, so we don't worry about "field is never assigned" warnings.
namespace Oceania_MG.Source
{
	class Block
	{
		public enum RenderType
		{
			Normal, //Always render full block
			Water, //Transparent
			ConnectedSolid, //Connect to any other solid block
			ConnectedSameType, //Only connect to blocks of same type
			ConnectedOre, //Takes texture of adjacent solid blocks, with overlay
		}

		private const int CORNERS = 4;
		private const float BACKGROUND_TINT = 0.6f;

		public string name;
		public string displayName;
		public string image;
		public string[] description = new string[0];
		public bool solid = true;
		public bool breakable = true;
		public RenderType renderType = RenderType.Normal;
		public string entity = "";
		public int harvestLevel = 0;
		public int breakTime = 100;
		public int id;
		private Texture2D texture;
		private Rectangle[][] cornerRects;
		private Func<Block, bool> connectedFunc;

		public void LoadImage(Texture2D texture)
		{
			this.texture = texture;
			if (renderType != RenderType.Normal)
			{
				cornerRects = new Rectangle[CORNERS][];
				for (int c = 0; c < CORNERS; c++)
				{
					cornerRects[c] = new Rectangle[2 * 2 * 2];
					bool[] values = new bool[] { false, true };
					foreach (bool a1 in values)
					{
						foreach (bool a2 in values)
						{
							foreach (bool a3 in values)
							{
								int i = GetAdjacent(a1, a2, a3);
								cornerRects[c][i] = GetSubRect(c, a1, a2, a3);
							}
						}
					}
				}

				if (renderType == RenderType.ConnectedSolid || renderType == RenderType.ConnectedOre)
				{
					connectedFunc = block => block.solid;
				}
				else if (renderType == RenderType.ConnectedSameType)
				{
					connectedFunc = block => block.id == id;
				}
			}
		}

		private Rectangle GetSubRect(int corner, bool adj1, bool adj2, bool adj3)
		{
			int mainX;
			int mainY;
			if (!adj1 && !adj3)
			{
				//don't care about corner adjacent block
				mainX = 0;
				mainY = 0;
			}
			else if (adj1 && adj2 && adj3)
			{
				mainX = 2;
				mainY = 0;
			}
			else if (adj1 && !adj2 && adj3)
			{
				mainX = 1;
				mainY = 1;
			}
			else
			{
				if ((corner % 2 == 1) == adj1)
				{
					mainX = 1;
					mainY = 0;
				}
				else
				{
					mainX = 0;
					mainY = 1;
				}
			}
			return GetSubRectFromPos(mainX, mainY, corner);
		}

		private Rectangle GetSubRectFromPos(int mainX, int mainY, int corner)
		{
			Vector2 cornerOffset = GetCornerOffset(corner);
			int subX = mainX * GameplayState.BLOCK_SIZE + (int)cornerOffset.X;
			int subY = mainY * GameplayState.BLOCK_SIZE + (int)cornerOffset.Y;

			Rectangle rect = new Rectangle(subX, subY, GameplayState.BLOCK_SIZE / 2, GameplayState.BLOCK_SIZE / 2);
			return rect;
		}

		private Vector2 GetCornerOffset(int corner)
		{
			Vector2 v = new Vector2(0, 0);
			if (corner == 1 || corner == 2)
			{
				v.X += GameplayState.BLOCK_SIZE / 2;
			}
			if (corner >= 2)
			{
				v.Y += GameplayState.BLOCK_SIZE / 2;
			}
			return v;
		}

		public void Draw(Vector2 pos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime, bool background, int worldX, int worldY, World world)
		{
			if (pos.X + GameplayState.SCALED_BLOCK_SIZE <= 0 || pos.X > Game.GetWidth() || pos.Y + GameplayState.SCALED_BLOCK_SIZE <= 0 || pos.Y > Game.GetHeight())
			{
				//block would be fully offscreen- don't render it
				return;
			}

			switch (renderType)
			{
				case RenderType.Normal:
					DrawTexture(pos, graphicsDevice, spriteBatch, gameTime, background, null, worldX, worldY, world);
					break;
				case RenderType.Water:
					//TODO: draw something?
					break;
				case RenderType.ConnectedOre:
					DrawOre(connectedFunc, pos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
					break;
				case RenderType.ConnectedSameType:
				case RenderType.ConnectedSolid:
					DrawConnectedTexture(connectedFunc, pos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
					break;
			}
		}

		private void DrawConnectedTexture(Func<Block, bool> connectFunc, Vector2 pos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime, bool background, int worldX, int worldY, World world)
		{
			for (int c = 0; c < CORNERS; c++)
			{
				//TODO simplify this
				int xOffset1 = 0, yOffset1 = 0, xOffset2 = 0, yOffset2 = 0, xOffset3 = 0, yOffset3 = 0;
				switch (c)
				{
					case 0:
						xOffset1 = -1;
						yOffset1 = 0;
						xOffset3 = 0;
						yOffset3 = -1;
						break;
					case 1:
						xOffset1 = 0;
						yOffset1 = -1;
						xOffset3 = 1;
						yOffset3 = 0;
						break;
					case 2:
						xOffset1 = 1;
						yOffset1 = 0;
						xOffset3 = 0;
						yOffset3 = 1;
						break;
					case 3:
						xOffset1 = 0;
						yOffset1 = 1;
						xOffset3 = -1;
						yOffset3 = 0;
						break;
				}
				xOffset2 = xOffset1 + xOffset3;
				yOffset2 = yOffset1 + yOffset3;

				bool a1 = connectFunc(world.GetBlockAt(worldX + xOffset1, worldY + yOffset1, background));
				bool a2 = connectFunc(world.GetBlockAt(worldX + xOffset2, worldY + yOffset2, background));
				bool a3 = connectFunc(world.GetBlockAt(worldX + xOffset3, worldY + yOffset3, background));

				int i = GetAdjacent(a1, a2, a3);
				Rectangle cornerRect = cornerRects[c][i];
				Vector2 subPos = pos + GetCornerOffset(c) * GameplayState.SCALE;
				DrawTexture(subPos, graphicsDevice, spriteBatch, gameTime, background, cornerRect, worldX, worldY, world);
			}
		}

		public Block GetSurroundingSolidBlock(int worldX, int worldY, bool background, World world)
		{
			Block[] surroundingBlocks = {
				world.GetBlockAt(worldX, worldY - 1, background),
				world.GetBlockAt(worldX, worldY + 1, background),
				world.GetBlockAt(worldX - 1, worldY, background),
				world.GetBlockAt(worldX + 1, worldY, background)
			};
			return surroundingBlocks.Where(x => x.solid).GroupBy(x => x).OrderByDescending(x => x.Count()).FirstOrDefault().Key; //TODO: null check?
		}

		private void DrawOre(Func<Block, bool> connectFunc, Vector2 pos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime, bool background, int worldX, int worldY, World world)
		{
			//TODO: make ore an entity, since this won't work for large groups
			//Find most frequent adjacent solid block, then render it under ore overlay texture
			Block surroundingBlock = GetSurroundingSolidBlock(worldX, worldY, background, world);
			if (surroundingBlock != null)
			{
				//surroundingBlock.Draw(pos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
			}
			DrawTexture(pos, graphicsDevice, spriteBatch, gameTime, background, null, worldX, worldY, world);
		}

		private void DrawTexture(Vector2 pos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime, bool background, Rectangle? sourceRect, int worldX, int worldY, World world)
		{
			Color color = world.GetLight(worldX, worldY);

			if (background)
			{

				color = new Color((int)(color.R * BACKGROUND_TINT), (int)(color.G * BACKGROUND_TINT), (int)(color.B * BACKGROUND_TINT), color.A);
			}

			if (name == "water")
			{
				color = Color.White;
			}

			spriteBatch.Draw(texture, pos, sourceRect, color, 0, Vector2.Zero, GameplayState.SCALE, SpriteEffects.None, 0);
		}

		private static int GetAdjacent(bool a1, bool a2, bool a3)
		{
			int i = 0;
			if (a1) i += 4;
			if (a2) i += 2;
			if (a3) i += 1;
			return i;
		}

		private bool HasConnectedTexture()
		{
			return renderType == RenderType.ConnectedSameType || renderType == RenderType.ConnectedSolid;
		}

		public void DrawSimple(SpriteBatch spriteBatch, Vector2 pos, float zoom, Rectangle? crop = null)
		{
			Rectangle sourceRect = new Rectangle(0, 0, GameplayState.BLOCK_SIZE, GameplayState.BLOCK_SIZE);
			if (crop.HasValue)
			{
				int cropX = (int)(crop.Value.X / zoom);
				int cropY = (int)(crop.Value.Y / zoom);
				Rectangle scaledCrop = new Rectangle(cropX, cropY, GameplayState.BLOCK_SIZE - cropX, GameplayState.BLOCK_SIZE - cropY);
				pos += ConvertUtils.PointToVector2(crop.Value.Location);
				sourceRect = Rectangle.Intersect(sourceRect, scaledCrop);
			}
			int textureOffset = HasConnectedTexture() ? 2 * GameplayState.BLOCK_SIZE : 0; //If connected texture, use the texture portion at the top-right
			sourceRect.X += textureOffset;
			spriteBatch.Draw(texture, pos, sourceRect, Color.White, 0, Vector2.Zero, zoom, SpriteEffects.None, 0);
		}
	}

	struct Blocks
	{
		public Block[] blocks;
	}
}
#pragma warning restore 0649
