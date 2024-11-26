using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Typoetry_WPF.ViewModels;

namespace Typoetry_WPF
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel(TypingTextBox);

            DataContext = _viewModel;

            TypingTextBox.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    e.Handled = true;
                }
            };

            TypingTextBox.PreviewDragOver += (s, e) => e.Handled = true;
            TypingTextBox.PreviewDrop += (s, e) => e.Handled = true;
        }
    }
}