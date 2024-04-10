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
    /// Interaction logic for AdminLogInWindow.xaml
    /// </summary>
    public partial class AdminLogInWindow : Window
    {
        private string Password = "admin1921";
        public AdminLogInWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(PasswordBox1.Password ==  Password)
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong Password");
            }
        }

        private void CanselButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
