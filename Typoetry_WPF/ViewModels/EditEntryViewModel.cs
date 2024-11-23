using System;
using System.Windows;
using System.Windows.Input;
using Typoetry.Models;
using Typoetry.Persistence;

namespace Typoetry_WPF.ViewModels
{
    public class EditEntryViewModel : ViewModelBase
    {
        private readonly Entry _entry;
        private readonly DataHandler _dataHandler;
        private readonly Action _refreshEntries;
        private readonly Window _window;

        private string _title;
        private string _author;
        private string _text;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                ValidateFields();
            }
        }

        public string Author
        {
            get => _author;
            set
            {
                _author = value;
                OnPropertyChanged();
                ValidateFields();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
                ValidateFields();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditEntryViewModel(Entry entry, DataHandler dataHandler, Action refreshEntries, Window window)
        {
            _entry = entry;
            _dataHandler = dataHandler;
            _refreshEntries = refreshEntries;
            _window = window;

            _title = entry.Title;
            _author = entry.Author;
            _text = entry.Text;

            SaveCommand = new DelegateCommand(CanExecuteSave, ExecuteSave);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteSave(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(Author) &&
                   !string.IsNullOrWhiteSpace(Text);
        }

        private void ExecuteSave(object? parameter)
        {
            try
            {
                _dataHandler.EditEntry(_entry.Id, Title.Trim(), Author.Trim(), Text.Trim());
                MessageBox.Show(
                    "Entry updated successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                _refreshEntries();
                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating entry: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            _window.Close();
        }

        private void ValidateFields()
        {
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
}