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
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PasswordBox1.Password == RepeatPassswordBox.Password)
                {
                    string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Visitor (Name, Password, Admin_id) VALUES (@Name, @Password, @Admin_id)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", LoginTexBox.Text);
                        command.Parameters.AddWithValue("@Password", PasswordBox1.Password);
                        command.Parameters.AddWithValue("@Admin_id", 1);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record inserted successfully");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to insert record");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Passwords are different");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
