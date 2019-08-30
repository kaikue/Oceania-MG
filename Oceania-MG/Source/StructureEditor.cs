using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oceania_MG.Source.GUI;
using Oceania_MG.Source.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
    /// <summary>
    /// GUI for editing structure files.
    /// </summary>
    public class StructureEditor : Microsoft.Xna.Framework.Game
    {
		private const string STRUCTURE_FILE_EXTENSION = ".struct";

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
		private const int PALETTE_WIDTH = 410;
		private const int ANCHOR_WIDTH = 195;
		private const int PROPERTIES_WIDTH = 195;
		private const int TEXT_BOX_HEIGHT = 25;

		private const int PALETTE_SPACING = 10;

		private static readonly Point TOOLTIP_OFFSET = new Point(12, 0);

		private Input input;
		private GUIContainer gui;
		private StructureEditPanel structureEditPanel;
		private TextBox nameTextBox;
		private TextBox freqTextBox;
		private TextBox minPerChunkTextBox;
		private TextBox maxPerChunkTextBox;
		private bool unsavedChanges;

		private SelectableBlock selectedBlock;
		private Dictionary<string, SelectableBlock> selectableBlocks;
		private bool backgroundActive;

		private string tooltip;

		private Resources resources;

		private static StructureEditor instance;

		public StructureEditor()
        {
			instance = this;

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

			Button newButton = new Button(new Rectangle(0, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "New", New, fileBar);
			gui.Add(newButton);

			Button openButton = new Button(new Rectangle(FILE_BUTTON_WIDTH, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "Open", Open, fileBar);
			gui.Add(openButton);

			Button saveButton = new Button(new Rectangle(2 * FILE_BUTTON_WIDTH, 0, FILE_BUTTON_WIDTH, FILE_BAR_HEIGHT), "Save", Save, fileBar);
			gui.Add(saveButton);

			//bottom bar (block palette, anchors, properties)
			ScrollPanel palette = new ScrollPanel(new Rectangle(0, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, PALETTE_WIDTH, EDIT_BAR_HEIGHT), "Blocks");
			gui.Add(palette);
			AddBlocks(palette);

			ScrollPanel anchors = new ScrollPanel(new Rectangle(PALETTE_WIDTH, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, ANCHOR_WIDTH, EDIT_BAR_HEIGHT), "Anchors");
			gui.Add(anchors);
			//TODO: add stuff to anchors

			ContainerPanel propertiesPanel = new ContainerPanel(new Rectangle(PALETTE_WIDTH + ANCHOR_WIDTH, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, PROPERTIES_WIDTH, EDIT_BAR_HEIGHT), "Properties");
			gui.Add(propertiesPanel);
			//TODO: add stuff to properties
			nameTextBox = new TextBox(new Rectangle(PALETTE_WIDTH + ANCHOR_WIDTH, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, PROPERTIES_WIDTH, TEXT_BOX_HEIGHT), propertiesPanel);
			//freqTextBox
			//minPerChunkTextBox
			//maxPerChunkTextBox

			//TODO: draw current layer button (foreground/background)
		}

		private void AddBlocks(ScrollPanel palette)
		{
			selectableBlocks = new Dictionary<string, SelectableBlock>();
			int x = PALETTE_SPACING;
			int y = PALETTE_SPACING;
			int blockSize = GameplayState.BLOCK_SIZE * GUIElement.scale;
			foreach (Block block in resources.GetBlocks())
			{
				SelectableBlock blockSelect = new SelectableBlock(new Rectangle(x, y, blockSize, blockSize), block, palette);
				Action selectAction = () =>
				{
					if (selectedBlock == blockSelect) return; //already selected
					if (selectedBlock != null) selectedBlock.Deselect();
					selectedBlock = blockSelect;
				};
				blockSelect.SetSelectAction(selectAction);

				selectableBlocks[block.name] = blockSelect;
				palette.AddScrollable(blockSelect);

				x += blockSize + PALETTE_SPACING;
				if (x + blockSize > palette.GetInnerBounds().Width)
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
			tooltip = null;

			input.Update();

			gui.Update(input);

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

			if (!string.IsNullOrEmpty(tooltip))
			{
				Tooltip.DrawTooltip(spriteBatch, tooltip, input.GetMousePosition() + TOOLTIP_OFFSET);
			}

			spriteBatch.End();

			base.Draw(gameTime);
        }

		private void ConfirmUnsavedChanges(Action action)
		{
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
			ConfirmUnsavedChanges(() =>
			{
				Reset();
			});
		}

		private void Open()
		{
			ConfirmUnsavedChanges(() =>
			{
				string filename = OpenFile();
				if (!string.IsNullOrEmpty(filename)) {
					Reset();
					Structure structure = SaveLoad.Load<Structure>(filename);
					structureEditPanel.Load(structure, resources);
				}
			});
		}

		private string OpenFile()
		{
			using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
			{
				openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\Content\Config\Structures";
				openFileDialog.Filter = "Structure files (*.struct)|*.struct|All files (*.*)|*.*";

				if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					return openFileDialog.FileName;
				}
				else
				{
					return null;
				}
			}
		}

		private void Reset()
		{
			structureEditPanel.Reset();
			//TODO: reset everything in menus
		}

		private void Save()
		{
			//save in thread so UI doesn't freeze
			Task.Run(() =>
			{
				Structure structure = structureEditPanel.Save();
				string filename = nameTextBox.text + STRUCTURE_FILE_EXTENSION;
				SaveLoad.Save(structure, filename);
				unsavedChanges = false;
			});
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			base.OnExiting(sender, args);

			if (unsavedChanges)
			{
				System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Save changes before exiting?", "Unsaved Changes", System.Windows.Forms.MessageBoxButtons.YesNo);
				if (dialogResult == System.Windows.Forms.DialogResult.Yes)
				{
					Save();
				}
			}
		}

		internal SelectableBlock GetSelectedBlock()
		{
			return selectedBlock;
		}

		public void SetSelectedBlock(string blockName)
		{
			selectableBlocks[blockName].Select();
		}

		public bool IsBackground()
		{
			return backgroundActive;
		}

		public void MarkChanged()
		{
			unsavedChanges = true;
		}

		public static void SetTooltip(string text)
		{
			instance.tooltip = text;
		}
    }

	class StructureEditPanel : GUIElement
	{
		private struct StructureBlockInfo
		{
			public Block block;
			public bool background;
			public int x;
			public int y;

			public StructureBlockInfo(Block block, int x, int y, bool background)
			{
				this.block = block;
				this.x = x;
				this.y = y;
				this.background = background;
			}
		}

		private Color hoverTintNormal = new Color(1.0f, 1.0f, 1.0f, 0.25f);
		private Color hoverTintErase = new Color(1.0f, 0.2f, 0.2f, 0.25f);

		//private Structure structure;
		private HashSet<StructureBlockInfo> blocks;

		private StructureEditor editor;

		private Point offset;

		private Point mousePos;
		private bool panning = false;
		private bool eraseMode = false;
		private float zoom = scale;
		private int zoomedBlockSize { get { return (int)(zoom * GameplayState.BLOCK_SIZE); } }

		public StructureEditPanel(Rectangle bounds, StructureEditor editor) : base(bounds)
		{
			blocks = new HashSet<StructureBlockInfo>();
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
			foreach(StructureBlockInfo blockInfo in blocks)
			{
				Vector2 pos = ConvertUtils.PointToVector2(GetBlockRenderPosition(new Point(blockInfo.x, blockInfo.y)));

				blockInfo.block.DrawSimple(spriteBatch, pos, zoom);

				//TODO: CTM rendering and background-layer tinting- use empty World()?
				//block.Draw(viewportPos, graphicsDevice, spriteBatch, new GameTime(), background, worldX, worldY, world);
			}

			if (hovered)
			{
				//draw highlight on hovered block space
				Rectangle hoverRect = new Rectangle(GetBlockRenderPosition(GetHoveredBlockPos()), new Point(zoomedBlockSize, zoomedBlockSize));
				Color hoverTint = eraseMode ? hoverTintErase : hoverTintNormal;
				spriteBatch.Draw(pixel, hoverRect, hoverTint);
			}
		}

		private Point GetHoveredBlockPos()
		{
			float fx = (float)(mousePos.X - offset.X) / zoomedBlockSize;
			int x = (int)Math.Floor(fx);

			float fy = (float)(mousePos.Y - offset.Y) / zoomedBlockSize;
			int y = (int)Math.Floor(fy);

			return new Point(x, y);
		}

		private StructureBlockInfo GetHoveredBlockInfo()
		{
			Point pos = GetHoveredBlockPos();
			return blocks.FirstOrDefault(block => block.x == pos.X && block.y == pos.Y && block.background == editor.IsBackground());
		}

		private Point GetBlockRenderPosition(Point blockPos)
		{
			return new Point(offset.X + blockPos.X * zoomedBlockSize, offset.Y + blockPos.Y * zoomedBlockSize);
		}

		public override void ControlPressed(Input.Controls control)
		{
			if (hovered) {
				if (control == Input.Controls.RightClick)
				{
					panning = true;
				}
				else if (control == Input.Controls.EditorPickBlock)
				{
					StructureBlockInfo blockInfo = GetHoveredBlockInfo();
					if (blockInfo.block != null)
					{
						editor.SetSelectedBlock(blockInfo.block.name);
					}
				}
			}
			if (control == Input.Controls.EditorErase)
			{
				eraseMode = true;
			}
		}

		public override void ControlHeld(Input.Controls control)
		{
			if (hovered)
			{
				if (control == Input.Controls.LeftClick)
				{
					if (eraseMode)
					{
						EraseBlock(GetHoveredBlockPos(), editor.IsBackground());
					}
					else
					{
						SelectableBlock selectedBlock = editor.GetSelectedBlock();
						if (selectedBlock != null)
						{
							SetBlock(GetHoveredBlockPos(), selectedBlock.GetBlock(), editor.IsBackground());
							editor.MarkChanged();
						}
					}
				}
			}
		}

		public override void ControlReleased(Input.Controls control)
		{
			if (control == Input.Controls.RightClick)
			{
				panning = false;
			}
			if (control == Input.Controls.EditorErase)
			{
				eraseMode = false;
			}
		}

		public void Reset()
		{
			blocks.Clear();
			offset = new Point(bounds.Width / 2, bounds.Height / 2); //start so that block position (0, 0) is in the center
		}

		private void SetBlock(Point point, Block block, bool background)
		{
			//clear all old blocks at position
			EraseBlock(point, background);

			blocks.Add(new StructureBlockInfo(block, point.X, point.Y, background));
		}

		private void EraseBlock(Point point, bool background)
		{
			blocks.RemoveWhere(block => block.x == point.X && block.y == point.Y && block.background == background);
		}

		public Structure Save()
		{
			Structure structure = new Structure();

			int minX = blocks.Min(block => block.x);
			int maxX = blocks.Max(block => block.x);
			int minY = blocks.Min(block => block.y);
			int maxY = blocks.Max(block => block.y);
			int width = maxX - minX + 1;
			int height = maxY - minY + 1;

			structure.blocksForeground = new string[height][];
			structure.blocksBackground = new string[height][];
			for (int i = 0; i < height; i++)
			{
				structure.blocksForeground[i] = new string[width];
				structure.blocksBackground[i] = new string[width];
			}

			foreach (StructureBlockInfo block in blocks)
			{
				int x = block.x - minX;
				int y = block.y - minY;
				string[][] blockArr = block.background ? structure.blocksBackground : structure.blocksForeground;
				blockArr[y][x] = block.block.name;
			}

			//TODO: anchors

			return structure;
		}

		public void Load(Structure structure, Resources resources)
		{
			Reset();

			//load all structure.blocks into blocks
			int height = structure.blocksForeground.Length;
			int width = structure.blocksForeground[0].Length;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					string blockFG = structure.blocksForeground[y][x];
					if (!string.IsNullOrEmpty(blockFG))
					{
						Block block = resources.GetBlock(blockFG);
						blocks.Add(new StructureBlockInfo(block, x, y, false));
					}

					string blockBG = structure.blocksBackground[y][x];
					if (!string.IsNullOrEmpty(blockBG))
					{
						Block block = resources.GetBlock(blockBG);
						blocks.Add(new StructureBlockInfo(block, x, y, true));
					}
				}
			}

			//TODO: load properties and anchors
		}
	}
}
