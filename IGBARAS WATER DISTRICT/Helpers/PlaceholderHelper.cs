using System;
using System.Drawing;
using System.Windows.Forms;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    public static class PlaceholderHelper
    {
        public static void AddPlaceholder(TextBox textBox, string placeholder, Color? placeholderColor = null, Color? textColor = null)
        {
            Color placeColor = placeholderColor ?? Color.Gray;
            Color normalColor = textColor ?? Color.Black;

            void SetPlaceholder()
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = placeColor;
                }
            }

            void RemovePlaceholder()
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = normalColor;
                }
            }

            textBox.GotFocus += (s, e) => RemovePlaceholder();
            textBox.LostFocus += (s, e) => SetPlaceholder();

            // Set initial state
            SetPlaceholder();
        }

        public static bool IsPlaceholder(TextBox textBox, string placeholder)
        {
            return textBox.Text == placeholder;
        }
    }
}
