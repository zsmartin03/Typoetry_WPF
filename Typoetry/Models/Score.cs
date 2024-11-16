namespace Typoetry.Models
{
    public class Score
    {
        public int Wpm { get; private set; }
        public int EntryId { get; private set; }
        public int TimeTakenSeconds { get; private set; }
        public int WrongCharacters { get; private set; }
        public DateTime Date { get; private set; }

        public Score(int wpm, int entryId, int timeTakenSeconds, int wrongCharacters)
        {
            Wpm = wpm;
            EntryId = entryId;
            TimeTakenSeconds = timeTakenSeconds;
            WrongCharacters = wrongCharacters;
            Date = DateTime.Now;
        }
    }
}
