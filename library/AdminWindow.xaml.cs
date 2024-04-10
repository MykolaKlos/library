using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True";

        public AdminWindow()
        {
            InitializeComponent();
            LoadAvailableBooks();
            LoadUsers();
            
            SortBookComboBox.Items.Add("Number of pages: ascending");
            SortBookComboBox.Items.Add("Number of pages: descending");
            SortBookComboBox.Items.Add("In alphabetical order");
            SortVisitorComboBox.Items.Add("Number of Books: ascending");
            SortVisitorComboBox.Items.Add("Number of Books: descending");
            SortVisitorComboBox.Items.Add("In alphabetical order");
        }


        private void LoadUsers()
        {
            try
            {
                List<Visitor> visitors = new List<Visitor>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Visitor";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Visitor visitor = new Visitor
                        {
                            id = (int)reader["id"],
                            Name = reader["Name"].ToString(),
                            Books = LoadBooksForVisitor((int)reader["id"])
                        };
                        visitors.Add(visitor);
                    }
                }

                VieversListBox.Items.Clear();
                VieversListBox.ItemsSource = visitors;
                VieversListBox.UpdateLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<Datum> LoadBooksForVisitor(int visitorId)
        {
            List<Datum> books = new List<Datum>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Datum WHERE Visitor_id = @VisitorId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@VisitorId", visitorId);
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
                    books.Add(book);
                }
            }

            return books;
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
                            Pages = (int)reader["Pages"],
                            handle = reader["handle"].ToString()

                        };
                        availableBooks.Add(book);
                    }
                }

                AvailableBoksListBoks.Items.Clear(); 
                AvailableBoksListBoks.ItemsSource = availableBooks;
                AvailableBoksListBoks.UpdateLayout();
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
            Created_atTextBlock.Text = $"Created at: ";
            HandleTextBlock.Text = $"Hendle: ";

            if (AvailableBoksListBoks.SelectedItem != null)
            {
                Datum selectedBook = (Datum)AvailableBoksListBoks.SelectedItem;
                NameTextBlock.Text += $" {selectedBook.Title}";
                AuthorTextBlock.Text += $" {selectedBook.Publisher}";
                YearTextBlock.Text += $" {selectedBook.Year.ToString()}";
                ISBNTextBlock.Text += $" {selectedBook.ISBN}";
                PagesTextBlock.Text += $" {selectedBook.Pages.ToString()}";
                Created_atTextBlock.Text += $" {selectedBook.created_at.ToString()}";
                HandleTextBlock.Text += $" {selectedBook.handle.ToString()}";
            }
        }

        private void RemoveBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableBoksListBoks.SelectedItem != null)
            {
                try
                {
                    Datum selectedBook = (Datum)AvailableBoksListBoks.SelectedItem;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Datum WHERE id = @BookId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@BookId", selectedBook.id);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Book successfully removed.");
                            ((List<Datum>)AvailableBoksListBoks.ItemsSource).Remove(selectedBook);
                        }
                        else
                        {
                            MessageBox.Show("Failed to remove the book.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow();
            addBookWindow.ShowDialog();

            if(addBookWindow.DialogResult == true)
            {
                AdminFactory adminFactory = AdminFactory.Instance;
                adminFactory.CreateNewBook(addBookWindow.Title, addBookWindow.Year, addBookWindow.Publisher, addBookWindow.ISBN, addBookWindow.Pages, null, null, connectionString);
                AvailableBoksListBoks.ItemsSource = null;
                LoadAvailableBooks(); 
            }
        }

        private void DeleteVisitorButton_Click(object sender, RoutedEventArgs e)
        {
            if (VieversListBox.SelectedItem != null)
            {
                try
                {
                    Visitor selectedVisitor = (Visitor)VieversListBox.SelectedItem;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string updateBooksQuery = "UPDATE Datum SET Visitor_id = NULL WHERE Visitor_id = @VisitorId";
                        SqlCommand updateBooksCommand = new SqlCommand(updateBooksQuery, connection);
                        updateBooksCommand.Parameters.AddWithValue("@VisitorId", selectedVisitor.id);

                        string deleteVisitorQuery = "DELETE FROM Visitor WHERE id = @VisitorId";
                        SqlCommand deleteVisitorCommand = new SqlCommand(deleteVisitorQuery, connection);
                        deleteVisitorCommand.Parameters.AddWithValue("@VisitorId", selectedVisitor.id);

                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        updateBooksCommand.Transaction = transaction;
                        deleteVisitorCommand.Transaction = transaction;

                        try
                        {
                            updateBooksCommand.ExecuteNonQuery();
                            deleteVisitorCommand.ExecuteNonQuery();
                            transaction.Commit();

                            List<Visitor> visitors = ((List<Visitor>)VieversListBox.ItemsSource);
                            visitors.Remove(selectedVisitor);
                            VieversListBox.ItemsSource = null; 
                            VieversListBox.ItemsSource = visitors; 
                            VieversListBox.UpdateLayout();

                            MessageBox.Show("Visitor successfully removed.");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Failed to remove the visitor. Error: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to remove the visitor. Error: " + ex.Message);
                }
            }
        }

        private void VieversListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IdVisitorTextBlock.Text = "Id: ";
            NameVisitorTextBlock.Text = "Name: ";
            BooksNumberTextBlock.Text = "Books number: ";

            if(VieversListBox.SelectedItem != null)
            {
                Visitor selectedVisitor = (Visitor)VieversListBox.SelectedItem;
                IdVisitorTextBlock.Text += selectedVisitor.id.ToString();
                NameVisitorTextBlock.Text += selectedVisitor.Name;
                BooksNumberTextBlock.Text += selectedVisitor.Books.ToList().Count.ToString();
            }
        }

        private void SortBookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortBookComboBox.SelectedItem != null)
            {
                string selectedSortCriteria = SortBookComboBox.SelectedItem.ToString();
                SortBooks(selectedSortCriteria);
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

        private void SearchBookTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBookTextBox.Text;

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

        private void SearchVisitorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchVisitorTextBox.Text;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Visitor WHERE Name LIKE @SearchText";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    List<Visitor> foundVisitors = new List<Visitor>();
                    while (reader.Read())
                    {
                        Visitor visitor = new Visitor
                        {
                            id = (int)reader["id"],
                            Name = reader["Name"].ToString(),
                            Books = new List<Datum>()
                        };
                        foundVisitors.Add(visitor);
                    }

                    VieversListBox.ItemsSource = foundVisitors;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SortVisitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortVisitorComboBox.SelectedItem != null)
            {
                string selectedSortCriteria = SortVisitorComboBox.SelectedItem.ToString();
                SortVisitors(selectedSortCriteria);
            }
        }

        private void SortVisitors(string sortCriteria)
        {
            List<Visitor> sortedVisitors = new List<Visitor>();

            switch (sortCriteria)
            {
                case "Number of Books: ascending":
                    sortedVisitors = ((List<Visitor>)VieversListBox.ItemsSource).OrderBy(visitor => visitor.Books.Count).ToList();
                    break;
                case "Number of Books: descending":
                    sortedVisitors = ((List<Visitor>)VieversListBox.ItemsSource).OrderByDescending(visitor => visitor.Books.Count).ToList();
                    break;
                case "In alphabetical order":
                    sortedVisitors = ((List<Visitor>)VieversListBox.ItemsSource).OrderBy(visitor => visitor.Name).ToList();
                    break;
            }

            VieversListBox.ItemsSource = sortedVisitors;
        }
    }
}
