using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace SimplePad.Behaviors
{
    /// <summary>
    /// This behavior stops the propagation of double-tapped events.
    /// </summary>
    public class StopDoubleTappedBehavior : Behavior<Control>
    {
        /// <summary>
        /// Override the OnAttached method to subscribe to the DoubleTapped event of the associated Control.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject == null) return;

            AssociatedObject.DoubleTapped += OnDoubleTapped;
        }

        /// <summary>
        /// Override the OnDetaching method to unsubscribe from the DoubleTapped event of the associated Control.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject == null) return;

            AssociatedObject.DoubleTapped -= OnDoubleTapped;
        }

        /// <summary>
        /// Handles the DoubleTapped event and stops its propagation by marking it as handled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
