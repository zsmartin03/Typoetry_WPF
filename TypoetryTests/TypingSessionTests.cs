using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Typoetry.Models;
using Typoetry.Persistence;
using Typoetry.Models.Exceptions;

namespace TypoetryTests.Models
{
    [TestClass]
    public class TypingSessionTests
    {
        private Mock<IDataHandler> mockDataHandler = null!;
        private TypingSession typingSession = null!;

        [TestInitialize]
        public void Setup()
        {
            mockDataHandler = new Mock<IDataHandler>();
            typingSession = new TypingSession(mockDataHandler.Object);
        }

        [TestMethod]
        public void SetNormalGame_SetsGameDataCorrectly()
        {
            // Arrange
            var testEntry = new Entry(1, "Test Title", "Test Author", "Test text");
            mockDataHandler.Setup(m => m.GetRandomEntry()).Returns(testEntry);

            // Act
            typingSession.SetNormalGame();

            // Assert
            Assert.AreEqual(testEntry.Id, typingSession.EntryId);
            Assert.AreEqual(testEntry.Text, typingSession.TextToWrite);
        }

        [TestMethod]
        public void SetCasualGame_SetsGameDataCorrectly()
        {
            // Arrange
            int numberOfWords = 5;
            var testEntry = new Entry(2, "Casual Game", "System", "This is a casual game");
            mockDataHandler.Setup(m => m.GetRandomWords(numberOfWords)).Returns(testEntry);

            // Act
            typingSession.SetCasualGame(numberOfWords);

            // Assert
            Assert.AreEqual(testEntry.Id, typingSession.EntryId);
            Assert.AreEqual(testEntry.Text, typingSession.TextToWrite);
        }

        [TestMethod]
        public void StartNewSession_ThrowsException_WhenTextToWriteIsEmpty()
        {
            // Act & Assert
            Assert.ThrowsException<EntryDataNotSetException>(() => typingSession.StartNewSession());
        }

        [TestMethod]
        public void StartNewSession_SetsInitialValuesCorrectly()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "Test text"));

            // Act
            typingSession.StartNewSession();

            // Assert
            Assert.IsTrue(typingSession.IsPlaying);
            Assert.AreEqual(0, typingSession.ElapsedTime);
            Assert.AreEqual(0, typingSession.CurrentPosition);
            Assert.AreEqual(0, typingSession.WrongCharacters);
            Assert.AreEqual(0, typingSession.Wpm);
        }

        [TestMethod]
        public void HandleKeyPress_CorrectKey_ReturnsTrue()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc"));
            typingSession.StartNewSession();

            // Act
            bool result = typingSession.HandleKeyPress('a');

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, typingSession.CurrentPosition);
            Assert.AreEqual(0, typingSession.WrongCharacters);
        }

        [TestMethod]
        public void HandleKeyPress_IncorrectKey_ReturnsFalse()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc"));
            typingSession.StartNewSession();

            // Act
            bool result = typingSession.HandleKeyPress('x');

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, typingSession.CurrentPosition);
            Assert.AreEqual(1, typingSession.WrongCharacters);
        }

        [TestMethod]
        public void HandleBackspace_CorrectUsage_ReturnsTrue()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc"));
            typingSession.StartNewSession();
            typingSession.HandleKeyPress('a');
            typingSession.HandleKeyPress('x'); // Incorrect key

            // Act
            bool result = typingSession.HandleBackspace();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, typingSession.CurrentPosition);
            Assert.AreEqual(0, typingSession.WrongCharacters);
        }

        [TestMethod]
        public void HandleBackspace_AtBeginning_ReturnsFalse()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc"));
            typingSession.StartNewSession();

            // Act
            bool result = typingSession.HandleBackspace();

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, typingSession.CurrentPosition);
        }

        [TestMethod]
        public void HandleEnter_CorrectUsage_ReturnsTrue()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc\ndef"));
            typingSession.StartNewSession();
            typingSession.HandleKeyPress('a');
            typingSession.HandleKeyPress('b');
            typingSession.HandleKeyPress('c');

            // Act
            bool result = typingSession.HandleEnter();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(4, typingSession.CurrentPosition);
        }

        [TestMethod]
        public void HandleEnter_IncorrectUsage_ReturnsFalse()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abcdef"));
            typingSession.StartNewSession();
            typingSession.HandleKeyPress('a');
            typingSession.HandleKeyPress('b');
            typingSession.HandleKeyPress('c');

            // Act
            bool result = typingSession.HandleEnter();

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(3, typingSession.CurrentPosition);
        }

        [TestMethod]
        public void EndSession_SavesScore()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "abc"));
            typingSession.StartNewSession();
            typingSession.HandleKeyPress('a');
            typingSession.HandleKeyPress('b');
            typingSession.HandleKeyPress('c');

            // Act
            typingSession.EndSession();

            // Assert
            mockDataHandler.Verify(m => m.SaveScore(It.IsAny<Score>()), Times.Once);
            Assert.IsFalse(typingSession.IsPlaying);
        }

        [TestMethod]
        public void CalculateWpm_CalculatesCorrectly()
        {
            // Arrange
            typingSession.SetChosenGame(new Entry(3, "Test Entry", "Test Author", "This is a test sentence."));
            typingSession.StartNewSession();

            // Simulate typing for 10 seconds
            for (int i = 0; i < "This is a test".Length; i++)
            {
                typingSession.HandleKeyPress(typingSession.TextToWrite[i]);
            }

            // Manually set elapsed time (since we can't easily simulate timer in unit test)
            typeof(TypingSession).GetProperty("ElapsedTime")?.SetValue(typingSession, 10);

            // Act
            typingSession.CalculateWpm();

            // Assert
            // 14 characters in 10 seconds = 84 characters per minute
            // 84 / 5 = 16.8 WPM (rounded to 16)
            Assert.AreEqual(16, typingSession.Wpm);
        }
    }
}