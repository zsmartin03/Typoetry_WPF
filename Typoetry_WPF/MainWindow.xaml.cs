using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Typoetry_WPF.ViewModels;

namespace Typoetry_WPF
{
    public partial class MainWindow : Window, IDisposable
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel(TypingTextBox);

            DataContext = _viewModel;
        }

        public void Dispose()
        {
            _viewModel.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _viewModel.Dispose();
        }
    }
}