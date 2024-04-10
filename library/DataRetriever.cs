using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace library
{
    public class DataRetriever
    {
        private string apiUrl = "https://stephen-king-api.onrender.com/api/books";
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True";

        public async Task RetrieveAndInsertDataAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        Rootobject data = JsonConvert.DeserializeObject<Rootobject>(json);
                        InsertDataIntoDatabase(data.data);
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve data from API");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InsertDataIntoDatabase(Datum[] data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var item in data)
                {
                    string query = "INSERT INTO Datum (Year, Title, handle, Publisher, ISBN, Pages, created_at) " +
                                   "VALUES (@Year, @Title, @Handle, @Publisher, @ISBN, @Pages, @Created_at)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Year", item.Year);
                    command.Parameters.AddWithValue("@Title", item.Title);
                    command.Parameters.AddWithValue("@Handle", item.handle);
                    command.Parameters.AddWithValue("@Publisher", item.Publisher);
                    command.Parameters.AddWithValue("@ISBN", item.ISBN);
                    command.Parameters.AddWithValue("@Pages", item.Pages);
                    command.Parameters.AddWithValue("@Created_at", item.created_at);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

