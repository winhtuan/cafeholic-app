using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace CAFEHOLIC.dao
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
    }
}
