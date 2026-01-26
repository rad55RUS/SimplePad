using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SimplePad.ViewModels;
using System;
using System.Threading.Tasks;
using static AvaloniaEdit.Document.TextDocumentWeakEventManager;


namespace SimplePad.Views
{
    public partial class SearchWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private MainViewModel? DefinedDataContext => (MainViewModel?)DataContext;

        /// <summary>
        /// 
        /// </summary>
        public SearchWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, RoutedEventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.F:
                        if (!e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                        {
                            SearchTabControl.SelectedIndex = 0;
                        }
                        else
                        {
                            SearchTabControl.SelectedIndex = 2;
                        }
                        e.Handled = true;
                        break;
                    case Key.H:
                        SearchTabControl.SelectedIndex = 1;
                        e.Handled = true;
                        break;
                    case Key.G:
                        SearchTabControl.SelectedIndex = 3;
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTabChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl)
            {
                switch (tabControl.SelectedIndex)
                {
                    case 0:
                        Height = 165;
                        break;
                    case 1:
                        Height = 229;
                        break;
                    case 2:
                        Height = 260;
                        break;
                    case 3:
                        Height = 127;
                        break;
                }
            }
        }
    }
}