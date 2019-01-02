using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
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
			public const string CONTROLLER_NAME = "Keyboard";

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
			public const string CONTROLLER_NAME = "Gamepad";

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

		class GamepadThumbstickControl : ControlType
		{
			public const string CONTROLLER_NAME = "GamepadThumbstick";

			private readonly bool left;

			public GamepadThumbstickControl(string stick)
			{
				string lower = stick.ToLower();
				if (lower != "left" && lower != "right")
				{
					throw new ArgumentException("Stick must be either Left or Right");
				}
				left = lower == "left";
			}

			public Vector2 GetValue()
			{
				return left ? GamePad.GetState(PlayerIndex.One).ThumbSticks.Left : GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
			}

			public override bool IsPressed()
			{
				return false;
			}
		}

		class MouseControl : ControlType
		{
			public const string CONTROLLER_NAME = "Mouse";

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
			Move,
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
			Hotbar1,
			Hotbar2,
			Hotbar3,
			Hotbar4,
			Hotbar5,
			Hotbar6,
			Hotbar7,
			Hotbar8,
			Hotbar9,
			Hotbar0,
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
			controlMappings = new Dictionary<Controls, HashSet<ControlType>>();
			
			foreach (Controls control in Enum.GetValues(typeof(Controls)))
			{
				controlMappings[control] = new HashSet<ControlType>();
			}

			string[] lines = File.ReadAllLines("Content/Config/controls.txt");
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				if (line.Length == 0 || line.StartsWith("//")) continue;
				string[] split = line.Split(':', '.');
				if (split.Length != 3) throw new FormatException("controls.txt line " + (i + 1) + " must be of format Action:Controller.Button");
				string actionStr = split[0].Trim(' ');
				string controllerStr = split[1].Trim(' ');
				string buttonStr = split[2].Trim(' ');

				bool success = Enum.TryParse(actionStr, out Controls control);
				if (!success) throw new FormatException("controls.txt line " + (i + 1) + ": Invalid action " + actionStr);

				if (controllerStr == KeyControl.CONTROLLER_NAME)
				{
					bool success2 = Enum.TryParse(buttonStr, out Keys key);
					if (!success2) throw new FormatException("controls.txt line " + (i + 1) + ": Invalid key " + buttonStr);
					AddKeyControl(control, key);
				}
				else if (controllerStr == GamepadControl.CONTROLLER_NAME)
				{
					bool success2 = Enum.TryParse(buttonStr, out Buttons button);
					if (!success2) throw new FormatException("controls.txt line " + (i + 1) + ": Invalid gamepad button " + buttonStr);
					AddGamepadControl(control, button);
				}
				else if (controllerStr == GamepadThumbstickControl.CONTROLLER_NAME)
				{
					AddGamepadThumbstickControl(control, buttonStr);
				}
				else if (controllerStr == MouseControl.CONTROLLER_NAME)
				{
					bool success2 = Enum.TryParse(buttonStr, out MouseButtons mouseButton);
					if (!success2) throw new FormatException("controls.txt line " + (i + 1) + ": Invalid mouse button " + buttonStr);
					AddMouseControl(control, mouseButton);
				}
				else
				{
					throw new FormatException("controls.txt line " + (i + 1) + ": Invalid controller " + controllerStr);
				}
			}
		}

		private void AddKeyControl(Controls control, Keys key)
		{
			controlMappings[control].Add(new KeyControl(key));
		}

		private void AddGamepadControl(Controls control, Buttons button)
		{
			controlMappings[control].Add(new GamepadControl(button));
		}

		private void AddGamepadThumbstickControl(Controls control, string stick)
		{
			controlMappings[control].Add(new GamepadThumbstickControl(stick));
		}

		private void AddMouseControl(Controls control, MouseButtons mouseButton)
		{
			controlMappings[control].Add(new MouseControl(mouseButton));
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

		/// <summary>
		/// Returns the first value of the control not equal to (0, 0), if one is defined using GamepadThumbstick. Returns Vector2(0, 0) otherwise.
		/// </summary>
		public Vector2 GetAxis(Controls control)
		{
			foreach (ControlType controlType in controlMappings[control])
			{
				if (controlType is GamepadThumbstickControl)
				{
					GamepadThumbstickControl thumbstickControl = (GamepadThumbstickControl)controlType;
					Vector2 value = thumbstickControl.GetValue();
					if (value.X != 0 || value.Y != 0)
					{
						return value;
					}
				}
			}
			return new Vector2(0, 0);
		}
	}
}
