using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace df_calculator
{
    class Program
    {
        static string connectionString = @"Data Source=tcp:178.217.40.162;Initial Catalog=SOFIndex;Integrated Security=False;User ID=arbitrclient;Password=159753;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        static void Main(string[] args)
        {
            Console.WriteLine("Term identifiactors recieving...");
            var ids = getTermIds();

            Console.WriteLine(ids.Count+" terms");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            int count = 0;

            foreach (var termId in ids)
            {
                SqlCommand cmd = new SqlCommand("update Terms set DocumentFrequency=(select count(TermId) from Postings where TermId=@termId) where Id=@termId",connection);
                cmd.Parameters.AddWithValue("termId", termId);

                cmd.ExecuteNonQuery();
                count++;

                if (count % 1000 == 0)
                {
                    Console.SetCursorPosition(0,2);
                    Console.WriteLine(count+" processed");
                }
            }
            Console.WriteLine("Complete");
            connection.Close();
        }

        static List<int> getTermIds()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("select Id from Terms", connection);
            connection.Open();

            List<int> result = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            connection.Close();

            return result;
        }
        
        
    }
}
