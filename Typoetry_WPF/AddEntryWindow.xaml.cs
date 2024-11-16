using System.Windows;
using Typoetry.Persistence;

namespace Typoetry_WPF
{
    public partial class AddEntryWindow : Window
    {
        private IDataHandler dh;

        public AddEntryWindow(IDataHandler dataHandler)
        {
            InitializeComponent();
            dh = dataHandler;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var title = txtTitle.Text.Trim();
            var author = txtAuthor.Text.Trim();
            var text = txtText.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("All fields must be filled.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dh.UploadText(title, author, text);
            MessageBox.Show("Entry added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
