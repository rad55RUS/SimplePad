using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;


namespace SimplePad.Views
{
    /// <summary>
    /// 
    /// </summary>
    static class ViewUtils
    {
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
    }
}
