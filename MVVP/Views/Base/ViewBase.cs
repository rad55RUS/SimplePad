using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace SimplePad.Views
{
    /// <summary>
    /// Represents the base class for all views in the application, providing common functionality and properties for derived views.
    /// </summary>
    public partial class ViewBase : UserControl
    {
        #region Event Handlers
        // AutoCompleteBox events
        /// <summary>
        /// Handles the event when the AutoCompleteBox is attached to the visual tree.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Focuses the inner TextBox of the AutoCompleteBox and selects all text within it, or focuses the AutoCompleteBox itself if the inner TextBox is not found.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void AutoCompleteBox_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is AutoCompleteBox autoCompleteBox)
            {
                autoCompleteBox.IsDropDownOpen = true;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var innerTextBox = autoCompleteBox.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();

                    if (innerTextBox != null)
                    {
                        innerTextBox.Focus();
                        innerTextBox.SelectAll();
                        return;
                    }

                    autoCompleteBox.Focus();

                }, DispatcherPriority.Input);
            }
        }

        /// <summary>
        /// Handles the event when the pointer is released on the AutoCompleteBox.
        /// <br/><br/>
        /// <b>Actions performed:</b>
        /// <br/>
        /// Opens the dropdown of the AutoCompleteBox if it is not already open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void AutoCompleteBox_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.Source is ContentPresenter) return;

            if (sender is AutoCompleteBox autoCompleteBox)
            {
                if (!autoCompleteBox.IsDropDownOpen)
                {
                    autoCompleteBox.IsDropDownOpen = true;
                }
            }
        }
        //
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the parent window of the current view by traversing the visual tree.
        /// </summary>
        /// <returns>The parent window, or null if not found.</returns>
        public Window? GetParentWindow()
        {
            return TopLevel.GetTopLevel(this) as Window;
        }
        #endregion
    }
}