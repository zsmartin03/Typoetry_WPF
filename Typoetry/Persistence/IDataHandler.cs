using System;
using System.Collections.Generic;
using Typoetry.Models;

namespace Typoetry.Persistence
{
    public interface IDataHandler
    {
        List<Entry> GetAllEntries();
        Entry GetRandomEntry();
        Entry GetEntryById(int id);
        void UploadText(string title, string author, string text);
        void EditEntry(int id, string newTitle, string newAuthor, string newText);
        void DeleteEntry(int id);
        List<string> GetTopWords();
        Entry GetRandomWords(int numberOfWords);
        void SaveScore(Score score);
        List<Score> GetAllScores();
        List<Score> GetTopScores(int top = 10);
    }
}
