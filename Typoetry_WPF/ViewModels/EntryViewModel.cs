using System.Windows;
using System.Windows.Input;
using Typoetry.Models;
using Typoetry.Persistence;
using Typoetry_WPF.Views;

namespace Typoetry_WPF.ViewModels
{
    public class EntryViewModel : ViewModelBase
    {
        private readonly Entry _entry;
        private readonly DataHandler _dataHandler;
        private readonly Action _refreshEntries;
        private readonly Action<Entry> _playEntry;

        public string Title => _entry.Title;
        public string Author => _entry.Author;
        public string TextPreview => GetTextPreview(_entry.Text, 100);

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand PlayCommand { get; }

        public EntryViewModel(Entry entry, DataHandler dataHandler, Action refreshEntries, Action<Entry> playEntry)
        {
            _entry = entry;
            _dataHandler = dataHandler;
            _refreshEntries = refreshEntries;
            _playEntry = playEntry;

            DeleteCommand = new DelegateCommand(ExecuteDelete);
            EditCommand = new DelegateCommand(ExecuteEdit);
            PlayCommand = new DelegateCommand(ExecutePlay);
        }

        private string GetTextPreview(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Length > maxLength ? $"{text.Substring(0, maxLength)}..." : text;
        }

        private void ExecuteDelete(object? parameter)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this entry?",
                "Delete Entry",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dataHandler.DeleteEntry(_entry.Id);
                    _refreshEntries();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while deleting the entry: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteEdit(object? parameter)
        {
            var editWindow = new EditEntryView();
            var editViewModel = new EditEntryViewModel(_entry, _dataHandler, _refreshEntries, editWindow);
            editWindow.DataContext = editViewModel;
            editWindow.Owner = Application.Current.MainWindow;
            editWindow.ShowDialog();
        }

        private void ExecutePlay(object? parameter)
        {
            _playEntry(_entry);
        }
    }
}