using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox1.Password;

            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name FROM Visitor WHERE Name = @Name AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", login);
                command.Parameters.AddWithValue("@Password", password);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int userId = reader.GetInt32(0);
                    string userName = reader.GetString(1);
                    VisitorWindow visitorWindow = new VisitorWindow(userId, userName);
                    visitorWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid login or password");
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AdminLogInWindow adminLogInWindow = new AdminLogInWindow();
            adminLogInWindow.Show();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow();
            regWindow.Show();

        }
    }
}