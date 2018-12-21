using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source
{
	public class Input
	{
		abstract class ControlType
		{
			public abstract bool IsPressed();
		}

		class KeyControl : ControlType
		{
			private readonly Keys key;

			public KeyControl(Keys key)
			{
				this.key = key;
			}

			public override bool IsPressed()
			{
				return Keyboard.GetState().IsKeyDown(key);
			}
		}

		class GamepadControl : ControlType
		{
			private readonly Buttons button;

			public GamepadControl(Buttons button)
			{
				this.button = button;
			}

			public override bool IsPressed()
			{
				return GamePad.GetState(PlayerIndex.One).IsButtonDown(button);
			}
		}

		class MouseControl : ControlType
		{
			private readonly MouseButtons mouseButton;
			private int scrollValue = 0;
			private int prevScrollValue = 0;

			public MouseControl(MouseButtons mouseButton)
			{
				this.mouseButton = mouseButton;
			}

			private ButtonState GetButtonState()
			{
				MouseState mouseState = Mouse.GetState();
				switch (mouseButton)
				{
					case MouseButtons.LeftClick:
						return mouseState.LeftButton;
					case MouseButtons.RightClick:
						return mouseState.RightButton;
					case MouseButtons.MiddleClick:
						return mouseState.MiddleButton;
					case MouseButtons.ScrollUp:
						return scrollValue > prevScrollValue ? ButtonState.Pressed : ButtonState.Released; //TODO handle integer overflow?
					case MouseButtons.ScrollDown:
						return scrollValue < prevScrollValue ? ButtonState.Pressed : ButtonState.Released; //TODO handle integer underflow?
					default:
						return ButtonState.Released;
				}
			}

			public override bool IsPressed()
			{
				//This should be called once per frame, so we can update scroll value here
				prevScrollValue = scrollValue;
				scrollValue = Mouse.GetState().ScrollWheelValue;

				return GetButtonState() == ButtonState.Pressed;
			}
		}

		enum MouseButtons
		{
			LeftClick,
			RightClick,
			MiddleClick,
			ScrollUp,
			ScrollDown,
		}

		public enum Controls
		{
			Up,
			Down,
			Left,
			Right,
			Break,
			Place,
			Background,
			HotbarPrev,
			HotbarNext,
			Inventory,
			Pause,
			Select,
		}

		private Dictionary<Controls, HashSet<ControlType>> controlMappings;
		
		private HashSet<Controls> heldControls; //for keys held indefinitely
		private HashSet<Controls> pressedControls; //for the one frame a key is first pressed
		private HashSet<Controls> releasedControls; //for the one frame a key is first unpressed

		public Input()
		{
			SetUpControlMappings();
			
			heldControls = new HashSet<Controls>();
			pressedControls = new HashSet<Controls>();
			releasedControls = new HashSet<Controls>();
		}

		private void SetUpControlMappings()
		{
			//TODO: initialize with config file???
			controlMappings = new Dictionary<Controls, HashSet<ControlType>>();
			
			foreach (Controls control in Enum.GetValues(typeof(Controls)))
			{
				controlMappings[control] = new HashSet<ControlType>();
			}

			//Movement
			AddKeyControl(Controls.Up, Keys.Up);
			AddKeyControl(Controls.Up, Keys.W);
			AddGamepadControl(Controls.Up, Buttons.LeftThumbstickUp);
			AddGamepadControl(Controls.Up, Buttons.DPadUp);
			AddKeyControl(Controls.Down, Keys.Down);
			AddKeyControl(Controls.Down, Keys.S);
			AddGamepadControl(Controls.Down, Buttons.LeftThumbstickDown);
			AddGamepadControl(Controls.Down, Buttons.DPadDown);
			AddKeyControl(Controls.Left, Keys.Left);
			AddKeyControl(Controls.Left, Keys.A);
			AddGamepadControl(Controls.Left, Buttons.LeftThumbstickLeft);
			AddGamepadControl(Controls.Left, Buttons.DPadLeft);
			AddKeyControl(Controls.Right, Keys.Right);
			AddKeyControl(Controls.Right, Keys.D);
			AddGamepadControl(Controls.Right, Buttons.LeftThumbstickRight);
			AddGamepadControl(Controls.Right, Buttons.DPadRight);

			//Actions
			AddMouseControl(Controls.Break, MouseButtons.LeftClick);
			AddGamepadControl(Controls.Break, Buttons.X);
			AddGamepadControl(Controls.Break, Buttons.RightTrigger);
			AddMouseControl(Controls.Place, MouseButtons.RightClick);
			AddGamepadControl(Controls.Place, Buttons.B);
			AddKeyControl(Controls.Background, Keys.LeftShift);
			AddKeyControl(Controls.Background, Keys.RightShift);
			AddGamepadControl(Controls.Background, Buttons.LeftTrigger);
			AddMouseControl(Controls.HotbarPrev, MouseButtons.ScrollUp);
			AddGamepadControl(Controls.HotbarPrev, Buttons.LeftShoulder);
			AddMouseControl(Controls.HotbarNext, MouseButtons.ScrollDown);
			AddGamepadControl(Controls.HotbarNext, Buttons.RightShoulder);
			AddKeyControl(Controls.Inventory, Keys.E);
			AddGamepadControl(Controls.Inventory, Buttons.Y);
			AddKeyControl(Controls.Pause, Keys.Escape);
			AddGamepadControl(Controls.Pause, Buttons.Start);
			AddMouseControl(Controls.Select, MouseButtons.LeftClick);
			AddKeyControl(Controls.Select, Keys.Enter);
			AddGamepadControl(Controls.Select, Buttons.A);
		}

		private void AddKeyControl(Controls action, Keys key)
		{
			controlMappings[action].Add(new KeyControl(key));
		}

		private void AddGamepadControl(Controls action, Buttons button)
		{
			controlMappings[action].Add(new GamepadControl(button));
		}

		private void AddMouseControl(Controls action, MouseButtons mouseButton)
		{
			controlMappings[action].Add(new MouseControl(mouseButton));
		}

		/// <summary>
		/// Update the input state.
		/// Should be called every frame.
		/// </summary>
		public void Update()
		{
			foreach (Controls control in controlMappings.Keys)
			{
				//check if of the corresponding control types are pressed
				bool isPressed = false;
				foreach (ControlType key in controlMappings[control])
				{
					if (key.IsPressed())
					{
						isPressed = true;
					}
				}

				if (isPressed) //key down
				{
					if (heldControls.Contains(control)) //was pressed last frame, so hold
					{
						pressedControls.Remove(control);
					}
					else //was not pressed last frame, so switch
					{
						heldControls.Add(control);
						pressedControls.Add(control);
						releasedControls.Remove(control); //in case it was released and pressed in 1 frame
					}
				}
				else //key up
				{
					if (heldControls.Contains(control)) //was pressed last frame, so switch
					{
						heldControls.Remove(control);
						pressedControls.Remove(control); //in case it was pressed and released in 1 frame
						releasedControls.Add(control);
					}
					else //was not pressed last frame, so hold
					{
						releasedControls.Remove(control);
					}
				}
			}
		}

		/// <summary>
		/// Returns true iff a key corresponding to the control is currently held down.
		/// </summary>
		public bool ControlHeld(Controls control)
		{
			return heldControls.Contains(control);
		}
		
		/// <summary>
		/// Returns true on the single frame that a key corresponding to the control has first been pressed.
		/// In the case of multiple keys corresponding to the same control, all keys must be released before KeyPressed will return true again.
		/// </summary>
		public bool ControlPressed(Controls control)
		{
			return pressedControls.Contains(control);
		}

		/// <summary>
		/// Returns true on the single frame that a key corresponding to the control has first been released.
		/// </summary>
		public bool ControlReleased(Controls control)
		{
			return releasedControls.Contains(control);
		}
	}
}
