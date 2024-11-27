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
        }
    }
}