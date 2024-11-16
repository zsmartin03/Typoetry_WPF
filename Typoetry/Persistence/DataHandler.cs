using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Typoetry.Models.Exceptions;
using Typoetry.Models;
using Typoetry.Persistence;

namespace Typoetry.Persistence
{
    public class DataHandler : IDataHandler
    {
        private string TextsFilePath;
        private string WordsFilePath;
        private string ScoresFilePath;

        private readonly JsonSerializerOptions jsonSerializerOptions;

        public DataHandler(string textsFilePath = "texts.json",
            string wordsFilePath = "top500words.txt", string scoresFilePath = "scores.json")
        {
            TextsFilePath = textsFilePath;
            WordsFilePath = wordsFilePath;
            ScoresFilePath = scoresFilePath;

            jsonSerializerOptions = new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public List<Entry> GetAllEntries()
        {
            if (!File.Exists(TextsFilePath))
                return new List<Entry>();

            string jsonString = File.ReadAllText(TextsFilePath);
            var entries = JsonSerializer.Deserialize<List<Entry>>(jsonString, jsonSerializerOptions);

            return entries ?? new List<Entry>();
        }

        public Entry GetRandomEntry()
        {
            var entries = GetAllEntries();
            if (entries == null || entries.Count == 0)
            {
                throw new InvalidOperationException("No entries available.");
            }

            Random random = new Random();
            int randomIndex = random.Next(entries.Count);

            return entries[randomIndex];
        }

        public Entry GetEntryById(int id)
        {
            if (id == 0)
            {
                return new Entry(0, "CASUAL", "", "");
            }
            var entries = GetAllEntries();

            var entry = entries.First(e => e.Id == id);

            return entry;
        }

        public void UploadText(string title, string author, string text)
        {
            var entries = GetAllEntries();
            int newId = entries.DefaultIfEmpty(new Entry(0, "", "", "")).Max(e => e.Id) + 1;

            var newEntry = new Entry(newId, title, author, text);
            entries.Add(newEntry);

            string jsonString = JsonSerializer.Serialize(entries, jsonSerializerOptions);
            File.WriteAllText(TextsFilePath, jsonString);
        }

        public void EditEntry(int id, string newTitle, string newAuthor, string newText)
        {
            var entries = GetAllEntries();

            var entry = entries.First(e => e.Id == id);

            if (entry == null)
            {
                throw new InvalidIdException(id);
            }

            entry.Title = newTitle;
            entry.Author = newAuthor;
            entry.Text = newText;

            string jsonString = JsonSerializer.Serialize(entries, jsonSerializerOptions);
            File.WriteAllText(TextsFilePath, jsonString);
        }

        public void DeleteEntry(int id)
        {
            var entries = GetAllEntries();
            var entry = entries.FirstOrDefault(e => e.Id == id);

            if (entry == null)
            {
                throw new InvalidIdException(id);
            }

            entries.Remove(entry);

            string jsonString = JsonSerializer.Serialize(entries, jsonSerializerOptions);
            File.WriteAllText(TextsFilePath, jsonString);

            // delete scores associated with the entry
            var scores = GetAllScores();
            var updatedScores = scores.Where(s => s.EntryId != id).ToList();

            string scoreJsonString = JsonSerializer.Serialize(updatedScores, jsonSerializerOptions);
            File.WriteAllText(ScoresFilePath, scoreJsonString);
        }


        public List<string> GetTopWords()
        {
            if (!File.Exists(WordsFilePath))
                throw new FileNotFoundException("Word file not found.");

            var words = File.ReadAllLines(WordsFilePath).ToList();
            return words;
        }

        public Entry GetRandomWords(int numberOfWords)
        {
            var words = GetTopWords();

            if (numberOfWords > words.Count)
                throw new ArgumentOutOfRangeException($"Requested {numberOfWords} words, but only {words.Count} are available.");

            Random random = new Random();
            var randomWords = words.OrderBy(x => random.Next()).Take(numberOfWords).ToList();

            return new Entry(0, "CASUAL", "", string.Join(" ", randomWords));
        }

        public void SaveScore(Score score)
        {
            List<Score> scores = GetAllScores();

            scores.Add(score);

            string jsonString = JsonSerializer.Serialize(scores, jsonSerializerOptions);
            File.WriteAllText(ScoresFilePath, jsonString);
        }

        public List<Score> GetAllScores()
        {
            if (!File.Exists(ScoresFilePath))
                return new List<Score>();

            string jsonString = File.ReadAllText(ScoresFilePath);
            var scores = JsonSerializer.Deserialize<List<Score>>(jsonString, jsonSerializerOptions);

            return scores ?? new List<Score>();
        }

        public List<Score> GetTopScores(int top = 10)
        {
            List<Score> scores = GetAllScores();

            var topScores = scores.OrderByDescending(s => s.Wpm)
                                  .Take(top)
                                  .ToList();

            return topScores;
        }
    }
}
