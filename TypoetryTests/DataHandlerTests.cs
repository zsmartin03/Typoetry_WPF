using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typoetry.Models;
using Typoetry.Models.Exceptions;
using Typoetry.Persistence;



namespace TypoetryTests.Persistence
{
    [TestClass]
    public class DataHandlerTests
    {
        private const string TestTextsFilePath = "test_texts.json";
        private const string TestScoresFilePath = "test_scores.json";
        private DataHandler dataHandler = new DataHandler(TestTextsFilePath, "top500words.txt", TestScoresFilePath);

        [TestInitialize]
        public void Setup()
        {

            // Clear test files before each test
            if (File.Exists(TestTextsFilePath))
                File.Delete(TestTextsFilePath);
            if (File.Exists(TestScoresFilePath))
                File.Delete(TestScoresFilePath);
        }

        [TestMethod]
        public void GetAllEntries_NoEntries_ReturnsEmptyList()
        {
            var entries = dataHandler.GetAllEntries();
            Assert.AreEqual(0, entries.Count);
        }

        [TestMethod]
        public void UploadText_AddsEntryToFile()
        {
            dataHandler.UploadText("Test Title", "Test Author", "Test Text");

            var entries = dataHandler.GetAllEntries();
            Assert.AreEqual(1, entries.Count);
            Assert.AreEqual("Test Title", entries[0].Title);
            Assert.AreEqual("Test Author", entries[0].Author);
            Assert.AreEqual("Test Text", entries[0].Text);
        }

        [TestMethod]
        public void GetRandomEntry_ThrowsException_WhenNoEntries()
        {
            Assert.ThrowsException<InvalidOperationException>(() => dataHandler.GetRandomEntry());
        }

        [TestMethod]
        public void GetEntryById_ReturnsCorrectEntry()
        {
            dataHandler.UploadText("Test Title", "Test Author", "Test Text");
            var entry = dataHandler.GetEntryById(1);

            Assert.IsNotNull(entry);
            Assert.AreEqual("Test Title", entry.Title);
            Assert.AreEqual("Test Author", entry.Author);
            Assert.AreEqual("Test Text", entry.Text);
        }

        [TestMethod]
        public void EditEntry_UpdatesEntrySuccessfully()
        {
            dataHandler.UploadText("Old Title", "Old Author", "Old Text");
            dataHandler.EditEntry(1, "New Title", "New Author", "New Text");

            var entry = dataHandler.GetEntryById(1);
            Assert.AreEqual("New Title", entry.Title);
            Assert.AreEqual("New Author", entry.Author);
            Assert.AreEqual("New Text", entry.Text);
        }

        [TestMethod]
        public void DeleteEntry_RemovesEntrySuccessfully()
        {
            dataHandler.UploadText("Test Title", "Test Author", "Test Text");
            dataHandler.DeleteEntry(1);

            var entries = dataHandler.GetAllEntries();
            Assert.AreEqual(0, entries.Count);
        }

        [TestMethod]
        public void SaveScore_AddsScoreToFile()
        {
            var score = new Score(100, 1, 30, 0);
            dataHandler.SaveScore(score);

            var scores = dataHandler.GetAllScores();
            Assert.AreEqual(1, scores.Count);
            Assert.AreEqual(100, scores[0].Wpm);
            Assert.AreEqual(1, scores[0].EntryId);
            Assert.AreEqual(30, scores[0].TimeTakenSeconds);
            Assert.AreEqual(0, scores[0].WrongCharacters);
        }

        [TestMethod]
        public void GetTopScores_ReturnsTopScores()
        {
            dataHandler.SaveScore(new Score(100, 1, 30, 0));
            dataHandler.SaveScore(new Score(200, 2, 20, 1));

            var topScores = dataHandler.GetTopScores();

            Assert.AreEqual(2, topScores.Count);
            Assert.AreEqual(200, topScores[0].Wpm);
            Assert.AreEqual(100, topScores[1].Wpm);
        }

        [TestMethod]
        public void GetTopWords_ThrowsException_WhenWordsFileNotFound()
        {
            Assert.ThrowsException<FileNotFoundException>(() => dataHandler.GetTopWords());
        }

    }
}