using Microsoft.Data.SqlClient;

namespace GoTaxi.DAL.Data
{
    public class DatabaseManager
    {
        private static SqlConnection connectionInstance = null;

        private DatabaseManager() { }

        public static SqlConnection GetInstance()
        {
            string connectionString = @"
                Server = .\SQLEXPRESS;
                Database = GoTaxiDB;
                Trusted_Connection=true;
                Integrated Security=true;
                TrustServerCertificate=true";

            try
            {
                if (connectionInstance == null)
                {
                    connectionInstance = new SqlConnection(connectionString);
                }

                return connectionInstance;
            }
            catch (SqlException exception)
            {
                Console.WriteLine(exception);
                return null;
            }

        }
    }
}