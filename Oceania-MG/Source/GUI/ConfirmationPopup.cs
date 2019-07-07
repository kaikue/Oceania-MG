using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oceania_MG.Source.GUI
{
	class ConfirmationPopup : PopupPanel
	{
		private const int BUTTON_HEIGHT = 25;
		private const int BUTTON_WIDTH = 75;
		private const int BUTTON_SPACING = 15;
		private const int BUTTON_VERTICAL_OFFSET = 40;

		public const int TOTAL_WIDTH = 4 * BUTTON_SPACING + 3 * BUTTON_WIDTH;
		public const int TOTAL_HEIGHT = BUTTON_VERTICAL_OFFSET + BUTTON_HEIGHT + BUTTON_SPACING;

		public ConfirmationPopup(Rectangle bounds, GUIContainer parentContainer, string label, Action yesAction, Action noAction) : base(bounds, parentContainer, label)
		{
			Button yesButton = new Button(new Rectangle(BUTTON_SPACING, BUTTON_VERTICAL_OFFSET, BUTTON_WIDTH, BUTTON_HEIGHT), "Yes", () => { yesAction(); Close(); }, parentContainer);
			Add(yesButton);

			Button noButton = new Button(new Rectangle(2 * BUTTON_SPACING + BUTTON_WIDTH, BUTTON_VERTICAL_OFFSET, BUTTON_WIDTH, BUTTON_HEIGHT), "No", () => { noAction(); Close(); }, parentContainer);
			Add(noButton);

			//if (cancelAction != null)
			//{
			Button cancelButton = new Button(new Rectangle(3 * BUTTON_SPACING + 2 * BUTTON_WIDTH, BUTTON_VERTICAL_OFFSET, BUTTON_WIDTH, BUTTON_HEIGHT), "Cancel", Close, parentContainer); //cancelAction);
			Add(cancelButton);
			//}
		}
	}
}
