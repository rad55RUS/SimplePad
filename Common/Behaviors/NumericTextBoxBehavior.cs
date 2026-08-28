using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using System.Linq;

namespace SimplePad.Behaviors
{
    /// <summary>
    /// This behavior allows only numeric input, functional keys, and navigation keys while preventing other non-numeric input.
    /// </summary>
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Override the OnAttached method to subscribe to the TextInput and KeyDown events of the associated TextBox control.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject == null) return;

            AssociatedObject.TextInput += OnTextInput;
            AssociatedObject.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Override the OnDetaching method to unsubscribe from the TextInput and KeyDown events of the associated TextBox control.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null) return;

            AssociatedObject.TextInput -= OnTextInput;
            AssociatedObject.KeyDown -= OnKeyDown;
        }

        /// <summary>
        /// Handles the TextInput event to prevent non-numeric input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text) && !e.Text.All(char.IsDigit))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Allows functional keys, navigation keys, and numeric keys while preventing other non-numeric input in the TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            // Allow functional key combinations
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control) ||
                e.KeyModifiers.HasFlag(KeyModifiers.Alt) ||
                e.KeyModifiers.HasFlag(KeyModifiers.Meta))
            {
                return;
            }
            //

            // Allow navigation
            if (e.Key == Key.Back ||
                e.Key == Key.Delete ||
                e.Key == Key.Left ||
                e.Key == Key.Right ||
                e.Key == Key.Up ||
                e.Key == Key.Down ||
                e.Key == Key.Home ||
                e.Key == Key.End ||
                e.Key == Key.PageUp ||
                e.Key == Key.PageDown ||
                e.Key == Key.Tab ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                return;
            }
            //

            // Allow functional keys
            if (e.Key >= Key.F1 && e.Key <= Key.F12)
            {
                return;
            }
            //

            // Allow main keyboard numbers
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                return;
            }
            //

            // Allow numpad numbers
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                return;
            }
            //

            e.Handled = true;
        }
    }
}
