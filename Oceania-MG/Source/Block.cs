using Microsoft.Xna.Framework;
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
		public enum ConnectedTexture
		{
			None, //Always render full block
			Solid, //Connect to any other solid block
			SameType, //Only connect to blocks of same type
		}

		private const int CORNERS = 4;

		public string name;
		public string displayName;
		public string image;
		public string[] description = new string[0];
		public bool solid = true;
		public bool breakable = true;
		public ConnectedTexture connectedTexture = ConnectedTexture.None;
		public string entity = "";
		public int harvestLevel = 0;
		public int breakTime = 100;
		public int id;
		private Texture2D texture;
		private Rectangle[][] cornerRects;
		private Func<Block, bool> connectedFunc;

		public void LoadImage()
		{
			texture = Game.LoadImage(image);

			if (connectedTexture != ConnectedTexture.None)
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

				switch (connectedTexture)
				{
					case ConnectedTexture.Solid:
						connectedFunc = block => block.solid;
						break;
					case ConnectedTexture.SameType:
						connectedFunc = block => block.id == id;
						break;
					default:
						break;
				}
			}
		}

		private Rectangle GetSubRect(int corner, bool adj1, bool adj2, bool adj3)
		{
			int mainX = 0;
			int mainY = 0;
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
			if (connectedTexture != ConnectedTexture.None)
			{
				DrawConnectedTexture(connectedFunc, pos, graphicsDevice, spriteBatch, gameTime, background, worldX, worldY, world);
			}
			else
			{
				Color color = world.GetLight(worldX, worldY);
				DrawTexture(texture, pos, graphicsDevice, spriteBatch, gameTime, color, background, null);
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

				bool a1 = connectFunc(world.BlockAt(worldX + xOffset1, worldY + yOffset1, background));
				bool a2 = connectFunc(world.BlockAt(worldX + xOffset2, worldY + yOffset2, background));
				bool a3 = connectFunc(world.BlockAt(worldX + xOffset3, worldY + yOffset3, background));

				Color color = world.GetLight(worldX, worldY);

				int i = GetAdjacent(a1, a2, a3);
				Rectangle cornerRect = cornerRects[c][i];
				Vector2 subPos = pos + GetCornerOffset(c) * GameplayState.SCALE;
				DrawTexture(texture, subPos, graphicsDevice, spriteBatch, gameTime, color, background, cornerRect);
			}
		}

		private void DrawTexture(Texture2D texture, Vector2 pos, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime, Color color, bool background, Rectangle? sourceRect)
		{
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
	}

	struct Blocks
	{
		public Block[] blocks;
	}
#pragma warning restore 0649
}
