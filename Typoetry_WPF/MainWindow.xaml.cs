using System.Windows;
using System.Windows.Controls;
using Typoetry.Models;
using Typoetry.Persistence;

namespace Typoetry_WPF
{
    public partial class MainWindow : Window
    {
        private DataHandler dh;
        private TypingSession session;

        public MainWindow()
        {
            InitializeComponent();

            dh = new DataHandler();
            session = new TypingSession(dh);

            // Attach event handlers
            btnPlayNormal.Click += BtnPlayNormal_Click;
            btnPlayCasual.Click += BtnPlayCasual_Click;
            btnChooseText.Click += BtnChooseText_Click;
            btnAddText.Click += BtnAddText_Click;

            rtbTypingText.PreviewKeyDown += KeyDownHandler;
            rtbTypingText.TextChanged += KeyPressed;
        }

        private void BtnPlayNormal_Click(object sender, RoutedEventArgs e)
        {
            session.SetNormalGame();
            StartTyping();
        }

        private void BtnPlayCasual_Click(object sender, RoutedEventArgs e)
        {
            session.SetCasualGame(50);
            StartTyping();
        }

        private void BtnChooseText_Click(object sender, RoutedEventArgs e)
        {
            // Display entries for selection
        }

        private void BtnAddText_Click(object sender, RoutedEventArgs e)
        {
            var addEntryView = new AddEntryWindow(dh);
            addEntryView.ShowDialog();
        }

        private void StartTyping()
        {
            rtbTypingText.IsEnabled = true;
            rtbTypingText.Focus();
        }

        private void KeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle key presses
        }

        private void KeyPressed(object sender, TextChangedEventArgs e)
        {
            // Handle typing logic
        }
    }
}
