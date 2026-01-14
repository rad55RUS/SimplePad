using Avalonia.Controls;
using Avalonia.Input;

namespace SimplePad.Views
{
    public partial class ViewBase : UserControl
    {
        public Window? GetParentWindow()
        {
            return TopLevel.GetTopLevel(this) as Window;
        }
    }
}