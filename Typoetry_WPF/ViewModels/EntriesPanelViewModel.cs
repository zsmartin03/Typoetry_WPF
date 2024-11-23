using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Typoetry.Models;
using Typoetry.Persistence;

namespace Typoetry_WPF.ViewModels
{
    public class EntriesPanelViewModel : ViewModelBase
    {
        private readonly DataHandler _dataHandler;
        private readonly Action<Entry> _playEntry;
        private ObservableCollection<EntryViewModel> _entries;

        public ObservableCollection<EntryViewModel> Entries
        {
            get => _entries;
            set
            {
                _entries = value;
                OnPropertyChanged();
            }
        }

        public EntriesPanelViewModel(DataHandler dataHandler, Action<Entry> playEntry)
        {
            _dataHandler = dataHandler;
            _playEntry = playEntry;
            _entries = new ObservableCollection<EntryViewModel>();
            LoadEntries();
        }

        public void LoadEntries()
        {
            var entries = _dataHandler.GetAllEntries();
            Entries.Clear();

            if (entries == null || entries.Count == 0)
            {
                MessageBox.Show("No entries found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var entry in entries)
            {
                Entries.Add(new EntryViewModel(entry, _dataHandler, LoadEntries, _playEntry));
            }
        }
    }
}
