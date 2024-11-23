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

            // Initialize ViewModel with the RichTextBox from XAML
            _viewModel = new MainViewModel(TypingTextBox);

            // Set DataContext for binding
            DataContext = _viewModel;

            // Disable copy/paste and drag/drop for the RichTextBox
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