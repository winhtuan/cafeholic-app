using System;
using System.Configuration;
using CAFEHOLIC.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using CAFEHOLIC;

namespace CAFEHOLIC.DAO
{
    public class DBContext
    {
        private readonly string connectionString;

        public DBContext()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["CafeDB"].ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load connection string.", ex);
            }
        }

        // Trả về một kết nối mới khi cần
        public SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
        public ILogger<T> GetLogger<T>()
        {
            return CAFEHOLIC.App.LoggerFactory.CreateLogger<T>();
        }

    }
}
