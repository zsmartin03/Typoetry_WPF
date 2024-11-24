using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Typoetry.Models;
using Typoetry.Persistence;

namespace Typoetry_WPF.ViewModels
{
    public class AddEntryViewModel : ViewModelBase
    {
        private readonly DataHandler _dataHandler;
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

        public AddEntryViewModel(DataHandler dataHandler, Window window)
        {
            
            _dataHandler = dataHandler;
            _window = window;

            _title = string.Empty;
            _author = string.Empty;
            _text = string.Empty;

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
                _dataHandler.UploadText(Title.Trim(), Author.Trim(), Text.Trim());
                MessageBox.Show(
                    "Entry added successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error adding entry: {ex.Message}",
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
