using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace library
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public string Title { get; set; }
        public DateTime Year { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public int Pages { get; set; }
        public DateTime Created_at { get; set; }
        public string Hendle {  get; set; }
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Title = TitleTextBox.Text;
            Publisher = PublisherTextBox.Text;
            Year = DateTime.Parse(YearTextBox.Text);
            ISBN = ISBNTextBox.Text;
            Pages = int.Parse(PagesTextBox.Text);
            Created_at = DateTime.Now;
            Hendle = Title.ToLower();
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}