using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace library
{

    public class Rootobject
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public DateTime Year { get; set; }
        public string Title { get; set; }
        public string handle { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public int Pages { get; set; }
        public string[] Notes { get; set; }
        public DateTime created_at { get; set; }
        public Villain[] villains { get; set; }
    }

    public class Villain
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Visitor
    {
        public Visitor()
        {
        }

        public Visitor(int id, string name, string password, List<Datum> books)
        {
            this.id = id;
            Name = name;
            Password = password;
            Books = books;
        }

        public int id { get; set; }
        public string Name { get; set; }
        private string Password { get;}
        public List<Datum> Books { get; set; }

    }

    public class Admin
    {
        private Admin(List<Datum> books, List<Visitor> visitors)
        {
            Books = books;
            Visitors = visitors;
        }
        public int id { get;}
        public List<Datum> Books { get; set; }
        public List<Visitor> Visitors { get; set; }
    }

    public class AdminFactory
    {
        private static AdminFactory instance;
        private AdminFactory() { }

        public static AdminFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdminFactory();
                }
                return instance;
            }
        }

        public void CreateNewBook(string title, DateTime year, string publisher, string isbn, int pages, string[] notes, Villain[] villains, string ConnectionString)
        {
            Datum book = new Datum
            {
                Title = title,
                Year = year,
                Publisher = publisher,
                ISBN = isbn,
                Pages = pages,
                Notes = notes,
                villains = villains
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Datum (Title, Year, Publisher, ISBN, Pages) VALUES (@Title, @Year, @Publisher, @ISBN, @Pages)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Publisher", publisher);
                    command.Parameters.AddWithValue("@ISBN", isbn);
                    command.Parameters.AddWithValue("@Pages", pages);
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Book added successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add book: " + ex.Message);
            }
        }
    }

}
