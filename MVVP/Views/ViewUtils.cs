using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using System;


namespace SimplePad.Views
{
    /// <summary>
    /// Provides utility methods for working with Avalonia views, including methods for positioning windows, retrieving context menu targets, and working with tree view items.
    /// </summary>
    public static class ViewUtils
    {
        #region Window methods
        /// <summary>
        /// Clamps the specified coordinates to the screen relative to the parent window.
        /// </summary>
        /// <param name="x">The x-coordinate to clamp.</param>
        /// <param name="y">The y-coordinate to clamp.</param>
        /// <param name="child">The child window.</param>
        /// <param name="parent">The parent window.</param>
        /// <returns>The clamped pixel point.</returns>
        public static PixelPoint ClampToScreenRelativeTo(int x, int y, Window child, Window parent)
        {
            Screen? currentScreen = parent.Screens.ScreenFromPoint(parent.Position) ?? parent.Screens.Primary;

            if (currentScreen != null)
            {
                PixelRect workingArea = currentScreen.WorkingArea;

                x = Math.Max(workingArea.X, Math.Min(x, workingArea.Right - (int)child.Width));
                y = Math.Max(workingArea.Y, Math.Min(y, workingArea.Bottom - (int)child.Height));
            }

            return new(x, y);
        }

        /// <summary>
        /// Gets the center position of the child window relative to the parent window.
        /// </summary>
        /// <param name="child">The child window.</param>
        /// <param name="parent">The parent window.</param>
        /// <returns>The pixel point representing the center position.</returns>
        public static PixelPoint GetCenterRelativeTo(Window child, Window parent)
        {
            int x = parent.Position.X + ((int)parent.Width - (int)child.Width) / 2;
            int y = parent.Position.Y + ((int)parent.Height - (int)child.Height) / 2;

            return ClampToScreenRelativeTo(x, y, child, parent);
        }

        /// <summary>
        /// Gets the position of the right-up corner of the child window relative to the parent window.
        /// </summary>
        /// <param name="child">The child window.</param>
        /// <param name="parent">The parent window.</param>
        /// <returns>The pixel point representing the right-up corner position.</returns>
        public static PixelPoint GetRightUpCornerRelativeTo(Window child, Window parent)
        {
            int x = parent.Position.X + ((int)parent.Width - (int)child.Width);
            int y = parent.Position.Y;

            return ClampToScreenRelativeTo(x, y, child, parent);
        }
        #endregion

        #region ContextMenu methods
        /// <summary>
        /// Gets the target control of the specified context menu by checking its parent popup.
        /// </summary>
        /// <param name="contextMenu">The context menu.</param>
        /// <returns>The target control, or null if not found.</returns>
        public static Control? GetContextMenuItemTarget(ContextMenu contextMenu)
        {
            if (contextMenu.Parent is Popup popup)
            {
                return popup.Parent as Control;
            }
            return null;
        }
        #endregion

        #region MenuFlyout methods
        /// <summary>
        /// Gets the target control of the specified menu flyout.
        /// </summary>
        /// <param name="menuFlyout">The menu flyout.</param>
        /// <returns>The target control, or null if not found.</returns>
        public static Control? GetMenuFlyoutTarget(MenuFlyout menuFlyout)
        {
            return menuFlyout.Target;

        }
        #endregion

        #region MenuItem methods
        /// <summary>
        /// Gets the target control of the specified menu item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns>The target control, or null if not found.</returns>
        public static Control? GetMenuItemTarget(MenuItem menuItem)
        {
            if (menuItem.Parent is MenuBase menu)
            {
                if (menu.Parent is Popup popup)
                {
                    return popup.Parent as Control;
                }
            }
            return null;
        }
        #endregion

        #region TreeView methods
        /// <summary>
        /// Selects the specified tree view item.
        /// </summary>
        /// <param name="elementItem">The tree view item to select.</param>
        public static void SelectTreeViewItem(StyledElement elementItem)
        {
            if (elementItem.Parent == null) return;
            if (elementItem.Parent is TreeViewItem treeViewItem)
            {
                TreeView? treeView = FindTreeViewItemParent(treeViewItem);
                if (treeView == null) return;

                treeView.UnselectAll();
                treeViewItem.IsSelected = true;
            }
            else
            {
                SelectTreeViewItem(elementItem.Parent);
            }
        }

        /// <summary>
        /// Finds the parent TreeView of the specified TreeViewItem by traversing up the visual tree.
        /// </summary>
        /// <param name="treeViewItem">The TreeViewItem to find the parent for.</param>
        /// <returns>The parent TreeView, or null if not found.</returns>
        public static TreeView? FindTreeViewItemParent(TreeViewItem? treeViewItem)
        {
            if (treeViewItem == null) return null;
            if (treeViewItem.Parent is TreeView)
            {
                return treeViewItem.Parent as TreeView;
            }
            else return FindTreeViewItemParent(treeViewItem.Parent as TreeViewItem);
        }

        /// <summary>
        /// Gets the TreeViewItem that contains the specified control by traversing up the visual tree.
        /// </summary>
        /// <param name="control">The control to find the parent TreeViewItem for.</param>
        /// <returns>The parent TreeViewItem, or null if not found.</returns>
        public static TreeViewItem? GetTreeViewItem(StyledElement control)
        {
            if (control.Parent is TreeViewItem treeViewItem)
            {
                return treeViewItem;
            }
            if (control.Parent == null)
            {
                return null;
            }

            return GetTreeViewItem(control.Parent);
        }
        #endregion

        #region Image methods
        /// <summary>
        /// Gets an SvgImage from the specified URI string by loading the SVG source.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <returns>The SvgImage.</returns>
        public static SvgImage GetSvgFromUriString(string uriString)
        {
            return new SvgImage { Source = SvgSource.Load(uriString, null) };
        }
        #endregion
    }
}