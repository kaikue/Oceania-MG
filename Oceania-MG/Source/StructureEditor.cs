using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Oceania_MG.Source.GUI;
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

		private Input input;
		private Structure structure;
		private GUIContainer gui;

		public StructureEditor()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			graphics.ApplyChanges();

			Window.Title = "Structure Editor";
		}
		
        protected override void Initialize()
        {
			base.Initialize();
			IsMouseVisible = true;
			input = new Input();

			//create empty structure
			structure = new Structure();

			//initialize GUI
			gui = new GUIContainer();

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
			Panel editBar = new Panel(new Rectangle(0, SCREEN_HEIGHT - EDIT_BAR_HEIGHT, SCREEN_WIDTH, EDIT_BAR_HEIGHT));
			gui.Add(editBar);

			//TODO: draw current layer button (foreground/background)
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

			/*if (input.ControlPressed(Input.Controls.LeftClick))
			{
				Console.WriteLine("Left click!");
			}*/

			gui.Update(input);

			/*float mouseX = (float)(Mouse.GetState().X / SCALE * 2 - WIDTH) / WIDTH;
			float mouseY = (float)(Mouse.GetState().Y / SCALE * 2 - HEIGHT) / HEIGHT;
			Biome hoverBiome = world.GetBiome(mouseX, mouseY, depth);
			hoverBiomeName = mouseX + ", " + mouseY + ": " + hoverBiome.name;*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);

			//TODO: draw structure itself
			//TODO: if mouse in structure region: highlight hovered block

			//TODO: draw top bar (new, load, save)

			//TODO: draw bottom bar (block palette, anchors, properties)

			//TODO: draw current layer button (foreground/background)

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

		private bool ConfirmUnsavedChanges()
		{
			//TODO: show a dialogue with "You have unsaved changes! Do you want to save?", options "Yes", "No", "Cancel"
			return true;
		}

		private void New()
		{
			Console.WriteLine("New pressed");
			if (ConfirmUnsavedChanges())
			{
				structure = new Structure();
				//TODO: reset everything in menus
			}
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
		}
    }
}
