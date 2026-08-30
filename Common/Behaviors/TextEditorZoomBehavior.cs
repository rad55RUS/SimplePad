using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;
using System;
using System.Linq;

namespace SimplePad.Behaviors
{
    /// <summary>
    /// This behavior allows zooming in and out of the text in a TextEditor control using the mouse wheel while holding down the Ctrl key.
    /// </summary>
    public class TextEditorZoomBehavior : Behavior<TextEditor>
    {
        private const double MIN_FONT_SIZE = 8;
        private const double MAX_FONT_SIZE = 72;
        private const double ZOOM_STEP = 2;

        /// <summary>
        /// Override the OnAttached method to subscribe to the PointerWheelChanged event of the associated TextEditor control.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.AddHandler(
                    InputElement.PointerWheelChangedEvent,
                    OnPointerWheelChanged,
                    Avalonia.Interactivity.RoutingStrategies.Tunnel);
            }
        }

        /// <summary>
        /// Override the OnDetaching method to unsubscribe from the PointerWheelChanged event of the associated TextEditor control.
        /// </summary>
        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.RemoveHandler(
                    InputElement.PointerWheelChangedEvent,
                    OnPointerWheelChanged);
            }

            base.OnDetaching();
        }

        /// <summary>
        /// Handles the PointerWheelChanged event to zoom in or out of the text in the TextEditor control when the Ctrl key is held down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (AssociatedObject == null) return;

            // Check if the Ctrl key is pressed
            var keyModifiers = e.KeyModifiers;
            if (!keyModifiers.HasFlag(KeyModifiers.Control)) return;
            //

            // Get the delta of the mouse wheel movement
            double delta = e.Delta.Y;
            if (delta == 0) return;
            //

            // Adjust the font size based on the wheel delta
            double fontSize = AssociatedObject.FontSize;

            if (delta > 0)
            {
                fontSize += ZOOM_STEP;
            }
            else
            {
                fontSize -= ZOOM_STEP;
            }
            //

            // Limit the font size
            fontSize = Math.Max(MIN_FONT_SIZE, Math.Min(MAX_FONT_SIZE, fontSize));
            //

            // Apply the new font size
            AssociatedObject.FontSize = fontSize;
            //

            // Prevent further event handling
            e.Handled = true;
            //
        }
    }
}