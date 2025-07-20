using CAFEHOLIC;
using CAFEHOLIC.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Data;

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
            try
            {
                var conn = new SqlConnection(connectionString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"[DBContext.GetConnection] Connection opened successfully, State: {conn.State}, ConnectionString: {conn.ConnectionString}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DBContext.GetConnection] Connection already open, reusing connection, State: {conn.State}");
                }
                return conn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DBContext.GetConnection] Error opening connection: {ex.Message}, InnerException: {ex.InnerException?.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        public ILogger<T> GetLogger<T>()
        {
            return CAFEHOLIC.App.LoggerFactory.CreateLogger<T>();
        }

    }
}
