using System;
using System.Text;
using System.Timers;
using Typoetry.Models.Exceptions;
using Typoetry.Persistence;
using Timer = System.Timers.Timer;

namespace Typoetry.Models
{
    public class TypingSession : IDisposable
    {
        public int EntryId { get; private set; }
        public string TextToWrite { get; private set; }
        public int Wpm { get; private set; }
        public bool IsPlaying { get; private set; }
        public int CurrentPosition { get; private set; }
        public int WrongCharacters { get; private set; }
        public int ElapsedTime { get; private set; }
        public string SessionOverview => IsPlaying ? $"Time: {ElapsedTime}s\nWPM: {Wpm}" : string.Empty;
        public string FinalOverview => $"Time needed: {ElapsedTime}s\nWPM: {Wpm}";
        public DateTime StartTime { get; private set; }

        private char[]? WrittenText;
        private IDataHandler DataHandler;
        private Timer TypingTimer;

        public event EventHandler? TimerTick;

        public TypingSession(IDataHandler dataHandler)
        {
            this.DataHandler = dataHandler;
            TextToWrite = string.Empty; 
            IsPlaying = false;
            
            TypingTimer = new Timer(1000);
            TypingTimer.Elapsed += TypingTimer_Elapsed;
        }

        private void SetGameData(Entry e)
        {
            TextToWrite = e.Text;
            EntryId = e.Id;

            WrittenText = new char[TextToWrite.Length];
        }

        public void SetNormalGame()
        {
            SetGameData(DataHandler.GetRandomEntry());
        }

        public void SetCasualGame(int numberOfWords)
        {
            SetGameData(DataHandler.GetRandomWords(numberOfWords));
        }

        public void SetChosenGame(Entry entry)
        {
            SetGameData(entry);
        }

        public void StartNewSession()
        {
            if (TextToWrite == string.Empty)
            {
                throw new EntryDataNotSetException();
            }
            IsPlaying = true;
            ElapsedTime = 0;
            CurrentPosition = 0;
            WrongCharacters = 0;
            StartTime = DateTime.Now;

            TypingTimer.Start();
        }

        private void TypingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            ElapsedTime++;
            CalculateWpm();
            TimerTick?.Invoke(this, EventArgs.Empty);
        }

        public void EndSession()
        {
            TerminateSession();
            Score score = new Score(Wpm, EntryId,
                ElapsedTime, WrongCharacters);
            DataHandler.SaveScore(score);
        }

        public void TerminateSession()
        {
            IsPlaying = false;
            WrittenText = null;
            TypingTimer.Stop();
        }


        public void CalculateWpm()
        {
            /*
             *  wpm (words per minute) 1 word = 5 chars: 
             *  ((characters written correctly / 5) / seconds passed) * 60
             */
            if (ElapsedTime != 0)
            {
                Wpm = (((CurrentPosition - WrongCharacters) * 60) / ElapsedTime) / 5;
            }
        }

        private void SetLastCharacter(char c)
        {
            if (WrittenText != null && CurrentPosition < WrittenText.Length) {
                WrittenText[CurrentPosition] = c;
            }
        }

        private bool IsLastCharacterCorrect()
        {
            if(WrittenText != null && CurrentPosition < WrittenText.Length)
            {
                if (WrittenText[CurrentPosition] == TextToWrite[CurrentPosition]) return true;
            }
            return false;
        }

        public bool HandleKeyPress(char pressedKey)
        {
            if (!IsPlaying) return false;

            SetLastCharacter(pressedKey);
            if (IsLastCharacterCorrect())
            {
                CurrentPosition++;
                return true;
            }
            else
            {
                WrongCharacters++;
                CurrentPosition++;
                return false;
            }
        }

        public bool HandleBackspace()
        {
            if (!IsPlaying || CurrentPosition == 0) return false;

            if(!IsLastCharacterCorrect())
            {
                WrongCharacters--;
            }
            CurrentPosition--;
            return true;
        }

        public bool HandleEnter()
        {
            if (!IsPlaying || CurrentPosition >= TextToWrite.Length) return false;
            if (TextToWrite[CurrentPosition] == '\n')
            {
                SetLastCharacter('\n');
                CurrentPosition++;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            TypingTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}