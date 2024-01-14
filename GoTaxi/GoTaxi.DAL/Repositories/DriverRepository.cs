using GoTaxi.DAL.Data;
using GoTaxi.DAL.Models;
using Microsoft.Data.SqlClient;

namespace GoTaxi.DAL.Repositories
{
    public class DriverRepository
    {
        private readonly SqlConnection _connection;

        private static DriverRepository instance = null;

        public static DriverRepository GetInstance()
        {
            if (instance == null)
            {
                instance = new DriverRepository();
            }

            return instance;
        }

        private DriverRepository()
        {
            _connection = DatabaseManager.GetInstance();
        }

        public Driver MapToDriver(SqlDataReader reader)
        {
            Driver driver = new Driver();

            driver.PlateNumber = reader.GetString(0);
            driver.Email = reader.GetString(1);
            driver.FullName = reader.GetString(2);
            driver.Password = reader.GetString(3);

            return driver;
        }

        // Read multiple
        public List<Driver> GetAllDrivers()
        {
            List<Driver> drivers = new List<Driver>();

            try
            {
                string query = "SELECT * FROM Drivers";

                _connection.Open();

                using SqlCommand command = new SqlCommand(query, _connection);


                SqlDataReader reader;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    drivers.Add(MapToDriver(reader));
                }

                return drivers;
            }
            catch (SqlException exception)
            {
                Console.WriteLine(exception);
                return null;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        // Read single
        public Driver GetDriverByPlateNumber(string plateNumber)
        {
            Driver driver = new Driver();

            try
            {
                string query = "SELECT * FROM Drivers WHERE PlateNumber = @PlateNumber";

                _connection.Open();

                SqlCommand command = new SqlCommand(query, _connection);

                command.Parameters.AddWithValue("PlateNumber", plateNumber);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    driver = MapToDriver(reader);
                }

                return driver;
            }
            catch (SqlException exception)
            {
                Console.WriteLine(exception);
                return null;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        // Create
        public void AddDriver(Driver newDriver)
        {
            try
            {
                string query = $"INSERT INTO [Drivers]([PlateNumber],[Email],[FullName],[Password]) VALUES(@PlateNumber,@Email,@FullName,@Password)";

                _connection.Open();

                SqlCommand command = new SqlCommand(query, _connection);

                command.Parameters.AddWithValue("PlateNumber", newDriver.PlateNumber);
                command.Parameters.AddWithValue("Email", newDriver.Email);
                command.Parameters.AddWithValue("FullName", newDriver.FullName);
                command.Parameters.AddWithValue("Password", newDriver.Password);

                command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        //Update
        public void UpdateDriver(Driver newDriver)
        {
            try
            {
                string query = $"UPDATE [Drivers] SET [Email] = @Email, [FullName] = @FullName, [Password] = @Password WHERE [PlateNumber] = @PlateNumber";

                _connection.Open();

                SqlCommand command = new SqlCommand(query, _connection);

                command.Parameters.AddWithValue("PlateNumber", newDriver.PlateNumber);
                command.Parameters.AddWithValue("Email", newDriver.Email);
                command.Parameters.AddWithValue("FullName", newDriver.FullName);
                command.Parameters.AddWithValue("Password", newDriver.Password);

                command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

    }
}