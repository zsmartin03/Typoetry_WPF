using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Typoetry.Models;
using Typoetry.Persistence;
using Typoetry_WPF.ViewModels;
using Typoetry_WPF.Views;

namespace Typoetry_WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DataHandler _dataHandler;
        private readonly TypingSession _session;
        private readonly DispatcherTimer _countdownTimer;
        private readonly RichTextBox _typingTextBox;

        private string _normalButtonText = "NORMAL";
        private string _countdownText = "";
        private string _sessionOverview = "";
        private string _leaderboardText = "";
        private EntriesPanelViewModel _entriesPanelViewModel;
        private Visibility _entriesPanelVisibility = Visibility.Collapsed;
        private Visibility _menuButtonsVisibility = Visibility.Visible;
        private Visibility _typingTextVisibility = Visibility.Collapsed;
        private Visibility _countdownVisibility = Visibility.Collapsed;
        private Visibility _overviewVisibility = Visibility.Collapsed;
        private Visibility _leaderboardVisibility = Visibility.Visible;

        public ICommand PlayNormalCommand { get; }
        public ICommand PlayCasualCommand { get; }
        public ICommand ChooseTextCommand { get; }
        public ICommand AddTextCommand { get; }

        public string NormalButtonText
        {
            get => _normalButtonText;
            set
            {
                _normalButtonText = value;
                OnPropertyChanged();
            }
        }

        public string CountdownText
        {
            get => _countdownText;
            set
            {
                _countdownText = value;
                OnPropertyChanged();
            }
        }

        public string SessionOverview
        {
            get => _sessionOverview;
            set
            {
                _sessionOverview = value;
                OnPropertyChanged();
            }
        }

        public string LeaderboardText
        {
            get => _leaderboardText;
            set
            {
                _leaderboardText = value;
                OnPropertyChanged();
            }
        }
        public EntriesPanelViewModel EntriesPanelViewModel
        {
            get => _entriesPanelViewModel;
            set
            {
                _entriesPanelViewModel = value;
                OnPropertyChanged();
            }
        }

        public Visibility EntriesPanelVisibility
        {
            get => _entriesPanelVisibility;
            set
            {
                _entriesPanelVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility MenuButtonsVisibility
        {
            get => _menuButtonsVisibility;
            set
            {
                _menuButtonsVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility TypingTextVisibility
        {
            get => _typingTextVisibility;
            set
            {
                _typingTextVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility CountdownVisibility
        {
            get => _countdownVisibility;
            set
            {
                _countdownVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility OverviewVisibility
        {
            get => _overviewVisibility;
            set
            {
                _overviewVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LeaderboardVisibility
        {
            get => _leaderboardVisibility;
            set
            {
                _leaderboardVisibility = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(RichTextBox typingTextBox)
        {
            _dataHandler = new DataHandler();
            _session = new TypingSession(_dataHandler);
            _typingTextBox = typingTextBox;

            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += CountdownTimer_Tick;

            PlayNormalCommand = new DelegateCommand(ExecutePlayNormal);
            PlayCasualCommand = new DelegateCommand(ExecutePlayCasual);
            ChooseTextCommand = new DelegateCommand(ExecuteChooseText);
            AddTextCommand = new DelegateCommand(ExecuteAddText);

            _session.TimerTick += Session_TimerTick;

            _typingTextBox.PreviewKeyDown += HandleSpecialKeys;
            _typingTextBox.PreviewTextInput += KeyPressed;

            ShowLeaderboard();
        }

        private void ExecuteAddText(object? parameter)
        {
            var addWindow = new AddEntryView();
            var addViewModel = new AddEntryViewModel(_dataHandler, addWindow);
            addWindow.DataContext = addViewModel;
            addWindow.Owner = Application.Current.MainWindow;
            addWindow.ShowDialog();
        }

        private void ExecuteChooseText(object? parameter)
        {
            LeaderboardVisibility = Visibility.Collapsed;
            EntriesPanelVisibility = Visibility.Visible;
            NormalButtonText = "BACK";
            ChangeMenuVisibility(false);

            if (EntriesPanelViewModel == null)
            {
                EntriesPanelViewModel = new EntriesPanelViewModel(_dataHandler, PlayChosenText);
            }
            else
            {
                EntriesPanelViewModel.LoadEntries();
            }
        }

        private void ExecutePlayNormal(object? parameter)
        {
            LeaderboardVisibility = Visibility.Collapsed;

            if (NormalButtonText == "NORMAL")
            {
                NormalButtonText = "BACK";
                _session.SetNormalGame();
                StartTyping();
            }
            else if (_session != null)
            {
                _session.TerminateSession();
                _countdownTimer.Stop();

                NormalButtonText = "NORMAL";
                ChangeMenuVisibility(true);
                ResetVisibility();
                ShowLeaderboard();
            }
        }

        private void ExecutePlayCasual(object? parameter)
        {
            LeaderboardVisibility = Visibility.Collapsed;
            _session.SetCasualGame(50);
            NormalButtonText = "BACK";
            ChangeMenuVisibility(false);
            StartTyping();
        }

        private void PlayChosenText(Entry entry)
        {
            _session.SetChosenGame(entry);
            EntriesPanelVisibility = Visibility.Collapsed;
            StartTyping();
        }


        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            int countdown = int.Parse(CountdownText);
            countdown--;

            if (countdown > 0)
            {
                CountdownText = countdown.ToString();
            }
            else
            {
                SetupTypingSession();
            }
        }

        private void SetupTypingSession()
        {
            _countdownTimer.Stop();
            CountdownVisibility = Visibility.Collapsed;

            _typingTextBox.Document.Blocks.Clear();
            _typingTextBox.Document.Blocks.Add(new Paragraph(new Run(_session.TextToWrite)));

            _session.StartNewSession();

            TypingTextVisibility = Visibility.Visible;
            OverviewVisibility = Visibility.Visible;
            SessionOverview = "Time: 0s\n\n";

            _typingTextBox.IsEnabled = true;
            _typingTextBox.Focus();
        }

        private void StartTyping()
        {
            ChangeMenuVisibility(false);
            CountdownText = "3";
            CountdownVisibility = Visibility.Visible;
            _typingTextBox.IsEnabled = false;
            _countdownTimer.Start();
        }

        private void Session_TimerTick(object? sender, EventArgs e)
        {
            SessionOverview = _session.SessionOverview;
        }

        private void HandleSpecialKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                HandleKey(' ', e);
            }
            else if(e.Key == Key.Enter)
            {
                HandleKey('\n', e);
            }
            else if (e.Key == Key.Back)
            {
                if (_session.HandleBackspace())
                {
                    ChangeCharacterBackgroundColor(_session.CurrentPosition + 1, new SolidColorBrush(Color.FromRgb(59, 66, 82)));
                }
                e.Handled = true;
            }
            else if(e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void KeyPressed(object sender, TextCompositionEventArgs e)
        {

            if (_session == null || !_session.IsPlaying) return;

            char pressedKey = e.Text[0];

            HandleKey(pressedKey);
        }

        private void HandleKey(char pressedKey, KeyEventArgs e = null)
        {

            if (_session.HandleKeyPress(pressedKey))
            {
                ChangeCharacterBackgroundColor(_session.CurrentPosition, new SolidColorBrush(Color.FromRgb(163, 190, 140)));
            }
            else
            {
                ChangeCharacterBackgroundColor(_session.CurrentPosition, new SolidColorBrush(Color.FromRgb(191, 97, 106)));
            }

            if (_session.CurrentPosition >= _session.TextToWrite.Length)
            {
                FinishTyping();
            }

            if (e != null) e.Handled = true;
        }
    

        private void ChangeCharacterBackgroundColor(int position, SolidColorBrush color)
        {
            var textRange = new TextRange(_typingTextBox.Document.ContentStart, _typingTextBox.Document.ContentEnd);
            string documentText = textRange.Text;

            if (position < 0 || position >= documentText.Length) return;

            TextPointer start = GetTextPointerAtCharacterOffset(_typingTextBox.Document.ContentStart, position - 1);
            TextPointer end = GetTextPointerAtCharacterOffset(_typingTextBox.Document.ContentStart, position);

            if (start != null && end != null)
            {
                var range = new TextRange(start, end);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, color);
            }
        }

        private TextPointer GetTextPointerAtCharacterOffset(TextPointer start, int characterOffset)
        {
            int currentOffset = 0;

            while (start != null)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var text = start.GetTextInRun(LogicalDirection.Forward);

                    if (currentOffset + text.Length > characterOffset)
                    {
                        return start.GetPositionAtOffset(characterOffset - currentOffset);
                    }

                    currentOffset += text.Length;
                }

                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            return null;
        }

        private void ChangeMenuVisibility(bool visible)
        {
            MenuButtonsVisibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ResetVisibility()
        {
            TypingTextVisibility = Visibility.Collapsed;
            OverviewVisibility = Visibility.Collapsed;
            CountdownVisibility = Visibility.Collapsed;
            EntriesPanelVisibility = Visibility.Collapsed;
        }

        private void ShowLeaderboard()
        {
            LeaderboardVisibility = Visibility.Visible;
            var scores = _dataHandler.GetTopScores(20);
            var leaderboardBuilder = new System.Text.StringBuilder();

            foreach (var score in scores)
            {
                var entry = _dataHandler.GetEntryById(score.EntryId);
                leaderboardBuilder.AppendLine($"| {score.Wpm} wpm - {entry.Title} |");
            }

            LeaderboardText = leaderboardBuilder.ToString();
        }


        private void FinishTyping()
        {
            _session.EndSession();
            MessageBox.Show(_session.FinalOverview, "Overview", MessageBoxButton.OK, MessageBoxImage.Information);

            OverviewVisibility = Visibility.Collapsed;
            ChangeMenuVisibility(true);
            TypingTextVisibility = Visibility.Collapsed;
            NormalButtonText = "NORMAL";

            ShowLeaderboard();
        }
    }
}