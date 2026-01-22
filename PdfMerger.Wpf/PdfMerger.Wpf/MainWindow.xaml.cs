using Microsoft.Win32;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Collections.ObjectModel;
using System.Windows;

namespace PdfMerger.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<string> _files = new();

        public MainWindow()
        {
            InitializeComponent();
            PdfList.ItemsSource = _files;
        }

        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (string file in dialog.FileNames)
                    _files.Add(file);
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfList.SelectedIndex;
            if (index > 0)
            {
                var item = _files[index];
                _files.RemoveAt(index);
                _files.Insert(index - 1, item);
                PdfList.SelectedIndex = index - 1;
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = PdfList.SelectedIndex;
            if (index >= 0 && index < _files.Count - 1)
            {
                var item = _files[index];
                _files.RemoveAt(index);
                _files.Insert(index + 1, item);
                PdfList.SelectedIndex = index + 1;
            }
        }

        private void Merge_Click(object sender, RoutedEventArgs e)
        {
            if (_files.Count == 0)
            {
                MessageBox.Show("Ingen PDF-filer valgt.");
                return;
            }

            SaveFileDialog saveDialog = new()
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "combined.pdf"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            PdfDocument output = new();

            foreach (string file in _files)
            {
                using PdfDocument input =
                    PdfReader.Open(file, PdfDocumentOpenMode.Import);

                foreach (PdfPage page in input.Pages)
                {
                    output.AddPage(page);
                }
            }

            output.Save(saveDialog.FileName);
            MessageBox.Show("PDF-filerne er nu samlet!");
        }
    }
}