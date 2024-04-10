using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for VisitorWindow.xaml
    /// </summary>
    public partial class VisitorWindow : Window
    {
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True";
        private int userId;
        private string userName;

        public VisitorWindow(int userId, string userName)
        {
            InitializeComponent();
            this.userId = userId;
            this.userName = userName;


            LoadUserBooks();
            LoadAvailableBooks();
            
            SortComboBox.Items.Add("Number of pages: ascending");
            SortComboBox.Items.Add("Number of pages: descending");
            SortComboBox.Items.Add("In alphabetical order");


            //DataRetriever dataRetriever = new DataRetriever();
            //dataRetriever.RetrieveAndInsertDataAsync();

            //testlabel1.Content = userName;
            //testlabel2.Content = userId;
        }
        private void LoadAvailableBooks()
        {
            try
            {
                List<Datum> availableBooks = new List<Datum>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Datum WHERE Visitor_id IS NULL";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Datum book = new Datum
                        {
                            id = (int)reader["id"],
                            Title = reader["Title"].ToString(),
                            Year = (DateTime)reader["Year"],
                            Publisher = reader["Publisher"].ToString(),
                            ISBN = reader["ISBN"].ToString(),
                            Pages = (int)reader["Pages"]
                        };
                        availableBooks.Add(book);
                    }
                }

                AvailableBoksListBoks.ItemsSource = availableBooks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadUserBooks()
        {
            try
            {
                List<Datum> userBooks = new List<Datum>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Datum WHERE Visitor_id = @UserId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Datum book = new Datum
                        {
                            id = (int)reader["id"],
                            Title = reader["Title"].ToString(),
                            Year = (DateTime)reader["Year"],
                            Publisher = reader["Publisher"].ToString(),
                            ISBN = reader["ISBN"].ToString(),
                            Pages = (int)reader["Pages"]
                        };
                        userBooks.Add(book);
                    }
                }

                VieverListBoks.ItemsSource = userBooks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void AvailableBoksListBoks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NameTextBlock.Text = "Title: ";
            AuthorTextBlock.Text = $"Publisher: ";
            YearTextBlock.Text = $"Year: ";
            ISBNTextBlock.Text = $"ISBN: ";
            PagesTextBlock.Text = $"Pages: ";

            if (AvailableBoksListBoks.SelectedItem != null)
            {
                Datum selectedBook = (Datum)AvailableBoksListBoks.SelectedItem;
                NameTextBlock.Text += $" {selectedBook.Title}";
                AuthorTextBlock.Text += $" {selectedBook.Publisher}";
                YearTextBlock.Text += $" {selectedBook.Year.ToString()}";
                ISBNTextBlock.Text += $" {selectedBook.ISBN}";
                PagesTextBlock.Text += $" {selectedBook.Pages.ToString()}";
            }
        }

        private void Take_a_bookButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableBoksListBoks.SelectedItem != null)
            {
                Datum selectedBook = (Datum)AvailableBoksListBoks.SelectedItem;
                ((List<Datum>)VieverListBoks.ItemsSource).Add(selectedBook);

                VieverListBoks.Items.Refresh();

                TakeBook(selectedBook.id, userId);
            }
        }

        private void TakeBook(int bookId, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Datum SET Visitor_id = @UserId WHERE id = @BookId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Book successfully taken.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to take the book.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void ReturnBook(int bookId, int? userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Datum SET Visitor_id = NULL WHERE id = @BookId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Book successfully returned.");
                        if (this.DataContext is Visitor user && user.Books != null)
                        {
                            Datum selectedBook = user.Books.FirstOrDefault(book => book.id == bookId);
                            if (selectedBook != null)
                            {
                                user.Books.Remove(selectedBook);
                                VieverListBoks.Items.Refresh();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to return the book.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Return_the_bookButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VieverListBoks.SelectedItem != null)
                {
                    Datum selectedBook = (Datum)VieverListBoks.SelectedItem;
                    ((List<Datum>)VieverListBoks.ItemsSource).Remove(selectedBook);
                    VieverListBoks.Items.Refresh();

                    ReturnBook(selectedBook.id, userId);
                    LoadAvailableBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Datum WHERE Title LIKE @SearchText";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    List<Datum> foundBooks = new List<Datum>();
                    while (reader.Read())
                    {
                        Datum book = new Datum
                        {
                            id = (int)reader["id"],
                            Title = reader["Title"].ToString(),
                            Year = (DateTime)reader["Year"],
                            Publisher = reader["Publisher"].ToString(),
                            ISBN = reader["ISBN"].ToString(),
                            Pages = (int)reader["Pages"]
                        };
                        foundBooks.Add(book);
                    }

                    AvailableBoksListBoks.ItemsSource = foundBooks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SortBooks(string sortCriteria)
        {
            List<Datum> sortedBooks = new List<Datum>();

            switch (sortCriteria)
            {
                case "Number of pages: ascending":
                    sortedBooks = ((List<Datum>)AvailableBoksListBoks.ItemsSource).OrderBy(book => book.Pages).ToList();
                    break;
                case "Number of pages: descending":
                    sortedBooks = ((List<Datum>)AvailableBoksListBoks.ItemsSource).OrderByDescending(book => book.Pages).ToList();
                    break;
                case "In alphabetical order":
                    sortedBooks = ((List<Datum>)AvailableBoksListBoks.ItemsSource).OrderBy(book => book.Title).ToList();
                    break;
            }

            AvailableBoksListBoks.ItemsSource = sortedBooks;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem != null)
            {
                string selectedSortCriteria = SortComboBox.SelectedItem.ToString();
                SortBooks(selectedSortCriteria);
            }
        }

      
    }
}
