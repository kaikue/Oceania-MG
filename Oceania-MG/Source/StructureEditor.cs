using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oceania_MG.Source.GUI;
using Oceania_MG.Source.States;
using System;

namespace Oceania_MG.Source
{
    /// <summary>
    /// GUI for editing structure files.
    /// </summary>
    public class StructureEditor : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Texture2D pixel;
		private SpriteFont font;

		private const int SCREEN_WIDTH = 800;
		private const int SCREEN_HEIGHT = 600;
		private const int SCALE = 2; //For things like text size, border width

		private const int FILE_BAR_HEIGHT = 25;
		private const int FILE_BUTTON_WIDTH = 75;
		private const int EDIT_BAR_HEIGHT = 150;

		private const int PALETTE_SPACING = 10;

		private Input input;
		private GUIContainer gui;
		private StructureEditPanel structureEditPanel;
		private bool unsavedChanges;

		private SelectableImage selectedBlock;

		internal Resources resources;

		public StructureEditor()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			graphics.ApplyChanges();

			Window.Title = "Structure Editor";
		}
		
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font/CodersCrux");
			
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(new Color[] { Color.White });

			GUIElement.scale = SCALE;
			GUIElement.font = font;
			GUIElement.pixel = pixel;

			resources = new Resources(this);
			resources.LoadBlocks();
		}

        protected override void Initialize()
        {
			base.Initialize();
			IsMouseVisible = true;
			input = new Input();

			//initialize GUI
			gui = new GUIContainer(new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));

			//structure editor
			structureEditPanel = new StructureEditPanel(new Rectangle(0, FILE_BAR_HEIGHT, SCREEN_WIDTH, SCREEN_HEIGHT - FILE_BAR_HEIGHT - EDIT_BAR_HEIGHT), this);
			gui.Add(structureEditPanel);

			//top bar (new, load, save)
			Panel fileBar = new Panel(new Rectangle(0, 0, SCREEN_WIDTH, FILE_BAR_HEIGHT));
			gui.Add(fileBar);

			Button newButton = new Button(new Rectangle(0, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "New", New);
			gui.Add(newButton);

			Button openButton = new Button(new Rectangle(FILE_BUTTON_WIDTH, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "Open", Open);
			gui.Add(openButton);

			Button saveButton = new Button(new Rectangle(2 * FILE_BUTTON_WIDTH, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "Save", Save);
			gui.Add(saveButton);

			//bottom bar (block palette, anchors, properties)
			ScrollPanel palette = new ScrollPanel(new Rectangle(0, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, SCREEN_WIDTH / 2, EDIT_BAR_HEIGHT), "Blocks");
			gui.Add(palette);
			AddBlocks(palette);
			//TODO: add stuff to palette

			ScrollPanel anchors = new ScrollPanel(new Rectangle(SCREEN_WIDTH / 2, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, SCREEN_WIDTH / 4, EDIT_BAR_HEIGHT), "Anchors");
			gui.Add(anchors);
			//TODO: add stuff to anchors

			ContainerPanel properties = new ContainerPanel(new Rectangle(SCREEN_WIDTH * 3 / 4, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, SCREEN_WIDTH / 4, EDIT_BAR_HEIGHT), "Properties");
			gui.Add(properties);
			//TODO: add stuff to properties

			//TODO: draw current layer button (foreground/background)
		}

		private void AddBlocks(ScrollPanel palette)
		{
			int x = PALETTE_SPACING;
			int y = PALETTE_SPACING; //TODO label offset?
			int blockSize = GameplayState.BLOCK_SIZE * GUIElement.scale;
			foreach (Block block in resources.GetBlocks())
			{
				Action selectAction = () =>
				{
					if (selectedBlock != null) selectedBlock.Deselect();
				};
				SelectableImage blockSelect = new SelectableImage(new Rectangle(x, y, blockSize, blockSize), block.GetTexture(), selectAction);
				selectAction += () =>
				{
					selectedBlock = blockSelect;
				};

				palette.Add(blockSelect);

				x += blockSize + PALETTE_SPACING;
				if (x > palette.GetBounds().Width)
				{
					x = PALETTE_SPACING;
					y += blockSize + PALETTE_SPACING;
				}
			}
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
			spriteBatch.Dispose();
			pixel.Dispose();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
        {
			/*if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				if (ConfirmUnsavedChanges())
				{
					Exit();
				}
			}*/

			input.Update();

			gui.Update(input);

			//TODO: don't activate these when editing text field
			//OR... make the control CTRL+whatever (needs input system change)
			if (input.ControlPressed(Input.Controls.EditorNew))
			{
				New();
			}
			if (input.ControlPressed(Input.Controls.EditorOpen))
			{
				Open();
			}
			if (input.ControlPressed(Input.Controls.EditorSave))
			{
				Save();
			}
			
			base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);

			/*for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					Color color = colors[depth - minDepth][x][y];
					spriteBatch.Draw(pixel, new Rectangle(x * SCALE, y * SCALE, SCALE, SCALE), color);
				}
			}

			spriteBatch.DrawString(font, "Depth: " + depth, new Vector2(10, 380), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			spriteBatch.DrawString(font, hoverBiomeName, new Vector2(10, 10), Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
			*/

			gui.Draw(spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
        }

		private void ConfirmUnsavedChanges(Action action)
		{
			unsavedChanges = true; //TODO: keep a flag for this

			if (unsavedChanges)
			{
				Action yes = () =>
				{
					Save();
					action();
				};
				Action no = () =>
				{
					action();
				};

				ConfirmationPopup popup = new ConfirmationPopup(new Rectangle(SCREEN_WIDTH / 2 - ConfirmationPopup.TOTAL_WIDTH / 2, 100, ConfirmationPopup.TOTAL_WIDTH, ConfirmationPopup.TOTAL_HEIGHT),
					gui, "Save unsaved changes?", yes, no);
				gui.Add(popup);
			}
			else
			{
				action();
			}
		}

		private void New()
		{
			Console.WriteLine("New pressed");
			ConfirmUnsavedChanges(() =>
			{
				structureEditPanel.Reset();
				//TODO: reset everything in menus
			});

		}

		private void Open()
		{
			Console.WriteLine("Open pressed");
			//TODO: open file picker dialogue and deserialize
		}

		private void Save()
		{
			Console.WriteLine("Save pressed");
			//TODO: serialize and write to file
			unsavedChanges = false;
		}
    }

	class StructureEditPanel : GUIElement
	{
		private Color hoverTint = new Color(1.0f, 1.0f, 1.0f, 0.5f);

		private Structure structure;

		private StructureEditor editor;

		private Point offset;

		private Point mousePos;
		private bool panning = false;
		private float zoom = scale;
		private int zoomedBlockSize { get { return (int)(zoom * GameplayState.BLOCK_SIZE); } }

		public StructureEditPanel(Rectangle bounds, StructureEditor editor) : base(bounds)
		{
			this.editor = editor;
			Reset();
		}

		public override void Update(Input input)
		{
			base.Update(input);

			Point oldMousePos = mousePos;

			mousePos = input.GetMousePosition();
			if (panning)
			{
				Point mouseMovement = mousePos - oldMousePos;
				offset += mouseMovement;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			bool[] layers = { true, false };
			foreach (bool background in layers)
			{
				for (int structureX = 0; structureX < structure.GetWidth(); structureX++)
				{
					for (int structureY = 0; structureY < structure.GetWidth(); structureY++)
					{
						Vector2 pos = ConvertUtils.PointToVector2(GetBlockRenderPosition(new Point(structureX, structureY)));

						string blockName = structure.GetBlockAt(structureX, structureY, background);
						Block block = editor.resources.GetBlock(blockName);

						int textureOffset = block.HasConnectedTexture() ? 2 * GameplayState.BLOCK_SIZE : 0; //If connected texture, use the texture portion at the top-right
						Rectangle sourceRect = new Rectangle(textureOffset, 0, GameplayState.BLOCK_SIZE, GameplayState.BLOCK_SIZE);
						spriteBatch.Draw(block.GetTexture(), pos, sourceRect, Color.White, 0, Vector2.Zero, zoom, SpriteEffects.None, 0);

						//TODO: CTM rendering and background-layer tinting- use empty World()?
						//block.Draw(viewportPos, graphicsDevice, spriteBatch, new GameTime(), background, worldX, worldY, world);
					}
				}
			}

			if (hovered)
			{
				//draw highlight on hovered block space
				Rectangle hoverRect = new Rectangle(GetBlockRenderPosition(GetHoveredBlock()), new Point(zoomedBlockSize, zoomedBlockSize));
				spriteBatch.Draw(pixel, hoverRect, hoverTint);
			}
		}

		private Point GetHoveredBlock()
		{
			float fx = (float)(mousePos.X - offset.X) / zoomedBlockSize;
			int x = (int)Math.Floor(fx);

			float fy = (float)(mousePos.Y - offset.Y) / zoomedBlockSize;
			int y = (int)Math.Floor(fy);

			return new Point(x, y);
		}

		private Point GetBlockRenderPosition(Point blockPos)
		{
			return new Point(offset.X + blockPos.X * zoomedBlockSize, offset.Y + blockPos.Y * zoomedBlockSize);
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (hovered) {
				if (control == Input.Controls.LeftClick)
				{
					//TODO: place according to the cursor state
					if (true)
					{

					}
				}
				else if (control == Input.Controls.RightClick)
				{
					panning = true;
				}
				else if (control == Input.Controls.MiddleClick)
				{
					//TODO pick block
				}
			}
		}

		public override void ControlReleased(Input.Controls control)
		{
			if (control == Input.Controls.RightClick)
			{
				panning = false;
			}
		}

		public void Reset()
		{
			structure = new Structure();
			offset = new Point(bounds.Width / 2, bounds.Height / 2); //start so that block position (0, 0) is in the center
		}
	}
}
