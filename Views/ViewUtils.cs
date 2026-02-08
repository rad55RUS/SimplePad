using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using Avalonia.Svg.Skia;
using System;


namespace SimplePad.Views
{
    /// <summary>
    /// 
    /// </summary>
    static class ViewUtils
    {
        #region Window methods
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
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PixelPoint GetCenterRelativeTo(Window child, Window parent)
        {
            int x = parent.Position.X + ((int)parent.Width - (int)child.Width) / 2;
            int y = parent.Position.Y + ((int)parent.Height - (int)child.Height) / 2;

            return ClampToScreenRelativeTo(x, y, child, parent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PixelPoint GetRightUpCornerRelativeTo(Window child, Window parent)
        {
            int x = parent.Position.X + ((int)parent.Width - (int)child.Width);
            int y = parent.Position.Y;

            return ClampToScreenRelativeTo(x, y, child, parent);
        }
        #endregion

        #region ContextMenu methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextMenu"></param>
        /// <returns></returns>
        public static Control? GetContextItemTarget(ContextMenu contextMenu)
        {
            if (contextMenu.Parent is Popup popup)
            {
                return popup.Parent as Control;
            }
            return null;
        }
        #endregion

        #region MenuItem methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="elementItem"></param>
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
        /// 
        /// </summary>
        /// <param name="treeViewItem"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public static SvgImage GetSvgFromUriString(string uriString)
        {
            return new SvgImage { Source = SvgSource.Load(uriString, null) };
        }
        #endregion
    }
}